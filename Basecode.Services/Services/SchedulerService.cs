using Basecode.Data.Dto;
using Basecode.Data.Models;
using Basecode.Data.ViewModels;
using Basecode.Services.Interfaces;
using Hangfire;
using NLog;

namespace Basecode.Services.Services;

public class SchedulerService : ErrorHandling, ISchedulerService
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly IApplicationService _applicationService;
    private readonly ICalendarService _calendarService;
    private readonly IExaminationService _examinationService;
    private readonly IInterviewService _interviewService;
    private readonly IScheduleSendingService _scheduleSendingService;
    private readonly IUserScheduleService _userScheduleService;
    private readonly IUserService _userService;

    public SchedulerService(IScheduleSendingService scheduleSendingService, IUserScheduleService userScheduleService,
        IApplicationService applicationService,
        IUserService userService, ICalendarService calendarService, IInterviewService interviewService,
        IExaminationService examinationService)
    {
        _scheduleSendingService = scheduleSendingService;
        _userScheduleService = userScheduleService;
        _applicationService = applicationService;
        _examinationService = examinationService;
        _interviewService = interviewService;
        _calendarService = calendarService;
        _userService = userService;
    }

    /// <summary>
    ///     Adds the schedules from the HR Scheduler.
    /// </summary>
    /// <param name="formData">The HR Scheduler form data.</param>
    public (LogContent, Dictionary<string, string>) AddSchedules(SchedulerDataViewModel formData, int userId)
    {
        (LogContent logContent, Dictionary<string, string> validationErrors) data = CheckSchedulerData(formData);

        if (!data.logContent.Result)
        {
            var newSchedules = new List<UserSchedule>();

            foreach (var schedule in formData.ApplicantSchedules)
            {
                var applicationId = _applicationService.GetApplicationIdByApplicantId(schedule.ApplicantId);

                var userSchedule = new UserSchedule
                {
                    UserId = userId,
                    ApplicationId = applicationId,
                    Type = formData.Type,
                    Schedule = DateTime.Parse(formData.Date + " " + schedule.Time),
                    Status = "pending"
                };

                var existingId = _userScheduleService.GetIdIfUserScheduleExists(applicationId);
                if (existingId != -1) // Existing schedule, change it from "rejected" to "pending"
                    HandleExistingSchedule(userSchedule, existingId, schedule.ApplicantId);
                else // New schedule, check it for errors
                    newSchedules = CheckNewSchedule(userSchedule, newSchedules);
            }

            if (newSchedules.Count > 0) // Add new schedules, if any
            {
                _userScheduleService.AddUserSchedules(newSchedules);
                _logger.Trace("New UserSchedule(s) have been successfully inserted into the database.");
            }

            foreach (var userSchedule in newSchedules)
            {
                var tokenExpiry = CalculateTokenExpiry(userSchedule.Schedule);
                _scheduleSendingService.SendScheduleToApplicant(userSchedule, userSchedule.Id, userSchedule.Type, tokenExpiry);
                CheckScheduleStatusAfterTokenExpiry(userSchedule.Id, userSchedule.Schedule, tokenExpiry);
            }

            _scheduleSendingService.SendSchedulesToInterviewer(formData, userId);
        }

        return data;
    }

    /// <summary>
    ///     Checks the schedule status after token expiry.
    /// </summary>
    /// <param name="userScheduleId">The user schedule identifier.</param>
    public void CheckScheduleStatus(int userScheduleId)
    {
        var userSchedule = _userScheduleService.GetUserScheduleById(userScheduleId);
        if (userSchedule != null && userSchedule.Status == "pending")
        {
            var logContent = RejectSchedule(userScheduleId);
            if (!logContent.Result)
                _logger.Trace(
                    $"Token has expired and was not used. UserSchedule [{userSchedule.Id}] has been automatically rejected.");
        }
    }

    /// <summary>
    ///     Accepts the schedule.
    /// </summary>
    /// <param name="userScheduleId">The user schedule identifier.</param>
    public async Task<LogContent> AcceptSchedule(int userScheduleId)
    {
        var userSchedule = _userScheduleService.GetUserScheduleById(userScheduleId);
        var logContent = CheckUserScheduleStatus(userSchedule);
        
        if (!logContent.Result)
        {
            var joinUrl = await SetOnlineMeetingSchedule(userSchedule);
            if (!string.IsNullOrEmpty(joinUrl))
            {
                _logger.Trace("Successfully generated a Teams meeting link.");

                _scheduleSendingService.SendAcceptedScheduleToInterviewer(userSchedule, joinUrl);
                _scheduleSendingService.SendAcceptedScheduleToApplicant(userSchedule, joinUrl);

                _userScheduleService.DeleteUserSchedule(userSchedule);
                if (userSchedule.Type == "For Final Interview")
                {
                    _scheduleSendingService.SendDecisionEmailToInterviewer(userSchedule);
                }
            }
            var data = new LogContent();
            var scheduleType = userSchedule.Type.Split(' ').Skip(1).FirstOrDefault();   // Leave only last word (Interview/Exam)

            if (scheduleType == "Interview")
                data = _interviewService.AddInterview(userSchedule, joinUrl);
            else
                data = _examinationService.AddExamination(userSchedule, joinUrl);

            if (!data.Result)
            {
                _logger.Trace($"Successfully created a new {userSchedule.Type} record.");

                userSchedule.Status = "accepted";
                _userScheduleService.UpdateUserSchedule(userSchedule);

                ScheduleApprovalOrReminderEmail(userSchedule, scheduleType);
            }
            else _logger.Error(SetLog(data)); 
        }

        return logContent;
    }

    /// <summary>
    /// Schedules the approval or reminder email.
    /// </summary>
    private void ScheduleApprovalOrReminderEmail(UserSchedule userSchedule, string scheduleType)
    {
        var hoursLeft = (int)(userSchedule.Schedule - DateTime.Now).TotalHours;
        // Send email on the hour of the schedule
        if (scheduleType == "Interview")
            _scheduleSendingService.ScheduleApprovalEmail(userSchedule, hoursLeft);
        else
            _scheduleSendingService.ScheduleExamScoreReminderEmail(userSchedule, hoursLeft);
    }

    /// <summary>
    /// Rejects the schedule.
    /// </summary>
    /// <param name="userScheduleId">The user schedule identifier.</param>
    public LogContent RejectSchedule(int userScheduleId)
    {
        var userSchedule = _userScheduleService.GetUserScheduleById(userScheduleId);
        var logContent = CheckUserScheduleStatus(userSchedule);

        if (!logContent.Result)
        {
            userSchedule.Status = "rejected";
            logContent = _userScheduleService.UpdateUserSchedule(userSchedule);
            _scheduleSendingService.SendRejectedScheduleNoticeToInterviewer(userSchedule);
        }

        return logContent;
    }

    /// <summary>
    ///     Sets the online meeting schedule.
    /// </summary>
    public async Task<string> SetOnlineMeetingSchedule(UserSchedule userSchedule)
    {
        var application = _applicationService.GetById(userSchedule.ApplicationId);
        var email = _userService.GetUserEmailById(userSchedule.UserId);

        var calendarEvent = new CalendarEvent
        {
            Subject = $"Alliance Software Inc. {userSchedule.Type}",
            Body = new Body
            {
                Content = $"{userSchedule.Type} for {application.JobOpeningTitle} position" +
                          $"<br> Applicant Name: {application.ApplicantName}"
            },
            Start = new EventDateTime { DateTime = userSchedule.Schedule },
            End = new EventDateTime { DateTime = userSchedule.Schedule.AddHours(1) }
        };

        var logContent = CheckCalendarEvent(calendarEvent);
        if (!logContent.Result)
            return await _calendarService.CreateEvent(calendarEvent, email);

        return "";
    }

    /// <summary>
    ///     Handles the existing schedule.
    /// </summary>
    private void HandleExistingSchedule(UserSchedule userSchedule, int existingId, int applicantId)
    {
        var logContent = _userScheduleService.UpdateUserSchedule(userSchedule, existingId);
        if (!logContent.Result)
        {
            _logger.Trace("Successfully updated User Schedule [ " + existingId + " ]");
            userSchedule.Id = existingId;
            var tokenExpiry = CalculateTokenExpiry(userSchedule.Schedule);
            _scheduleSendingService.SendScheduleToApplicant(userSchedule, existingId, userSchedule.Type, tokenExpiry, applicantId);
            CheckScheduleStatusAfterTokenExpiry(userSchedule.Id, userSchedule.Schedule, tokenExpiry);
        }
    }

    /// <summary>
    ///     Handles the new schedule.
    /// </summary>
    private List<UserSchedule> CheckNewSchedule(UserSchedule userSchedule, List<UserSchedule> newSchedules)
    {
        var logContent = CheckUserSchedule(userSchedule);
        if (!logContent.Result) newSchedules.Add(userSchedule);
        return newSchedules;
    }

    /// <summary>
    /// Checks the schedule status after token expiry.
    /// </summary>
    /// <param name="userScheduleId">The user schedule identifier.</param>
    /// <param name="schedule">The schedule.</param>
    private void CheckScheduleStatusAfterTokenExpiry(int userScheduleId, DateTime schedule, int tokenExpiry)
    {
        BackgroundJob.Schedule(() => CheckScheduleStatus(userScheduleId), TimeSpan.FromHours(tokenExpiry));
    }

    /// <summary>
    /// Calculates the token expiry.
    /// </summary>
    /// <param name="schedule">The schedule.</param>
    /// <returns></returns>
    private int CalculateTokenExpiry(DateTime schedule)
    {
        var timeDifference = schedule - DateTime.Now;

        // If the schedule is within 2 days (rush meeting), don't expire too quickly.
        if (timeDifference.TotalDays <= 2)
        {
            return Math.Max(2, (int)timeDifference.TotalHours);
        }
        else
        {
            // For non-rush meetings, expire at most 12 hours before the schedule.
            var expiryTime = schedule.AddHours(-12);
            var hoursLeft = (int)(expiryTime - DateTime.Now).TotalHours;
            return hoursLeft > 0 ? hoursLeft : 0;
        }
    }
}