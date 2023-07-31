using AutoMapper;
using Basecode.Data.Dto;
using Basecode.Data.Models;
using Basecode.Data.ViewModels;
using Basecode.Services.Interfaces;
using Hangfire;
using NLog;

namespace Basecode.Services.Services
{
    public class SchedulerService : ErrorHandling, ISchedulerService
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IUserScheduleService _userScheduleService;
        private readonly IEmailSendingService _emailSendingService;
        private readonly IApplicationService _applicationService;
        private readonly IExaminationService _examinationService;
        private readonly IJobOpeningService _jobOpeningService;
        private readonly IInterviewService _interviewService;
        private readonly IApplicantService _applicantService;
        private readonly ICalendarService _calendarService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public SchedulerService(IUserScheduleService userScheduleService, IApplicationService applicationService, IEmailSendingService emailSendingService, IApplicantService applicantService, IUserService userService, 
            ICalendarService calendarService, IInterviewService interviewService, IExaminationService examinationService, IJobOpeningService jobOpeningService, IMapper mapper)
        {
            _userScheduleService = userScheduleService;
            _emailSendingService = emailSendingService;
            _applicationService = applicationService;
            _examinationService = examinationService;
            _jobOpeningService = jobOpeningService;
            _interviewService = interviewService;
            _applicantService = applicantService;
            _calendarService = calendarService;
            _userService = userService;   
            _mapper = mapper;
        }

        /// <summary>
        /// Adds the schedules from the HR Scheduler.
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
                    Guid applicationId = _applicationService.GetApplicationIdByApplicantId(schedule.ApplicantId);

                    var userSchedule = new UserSchedule
                    {
                        UserId = userId,
                        ApplicationId = applicationId,
                        Type = formData.Type,
                        Schedule = DateTime.Parse(formData.Date.ToString() + " " + schedule.Time),
                        Status = "pending",
                    };

                    int existingId = _userScheduleService.GetIdIfUserScheduleExists(applicationId);
                    if (existingId != -1)   // Existing schedule, change it from "rejected" to "pending"
                        HandleExistingSchedule(userSchedule, existingId, schedule.ApplicantId);
                    else    // New schedule, check it for errors
                        newSchedules = CheckNewSchedule(userSchedule, newSchedules);
                }

                if (newSchedules.Count > 0)     // Add new schedules, if any
                {
                    _userScheduleService.AddUserSchedules(newSchedules);
                    _logger.Trace("New UserSchedule(s) have been successfully inserted into the database.");
                }
                    
                foreach (var userSchedule in newSchedules)
                    SendScheduleToApplicant(userSchedule, userSchedule.Id, userSchedule.Type);

                SendSchedulesToInterviewer(formData, userId);
            }

            return data;
        }

        /// <summary>
        /// Handles the existing schedule.
        /// </summary>
        private void HandleExistingSchedule(UserSchedule userSchedule, int existingId, int applicantId)
        {
            LogContent logContent = _userScheduleService.UpdateUserSchedule(userSchedule, existingId);
            if (!logContent.Result)
            {
                _logger.Trace("Successfully updated User Schedule [ " + existingId + " ]");
                userSchedule.Id = existingId;
                SendScheduleToApplicant(userSchedule, existingId, userSchedule.Type, applicantId);
            }
        }

        /// <summary>
        /// Handles the new schedule.
        /// </summary>
        private List<UserSchedule> CheckNewSchedule(UserSchedule userSchedule, List<UserSchedule> newSchedules)
        {
            LogContent logContent = CheckUserSchedule(userSchedule);
            if (!logContent.Result) newSchedules.Add(userSchedule);
            return newSchedules;
        }

        /// <summary>
        /// Sends the schedule to applicant.
        /// </summary>
        /// <param name="userSchedule">The user schedule.</param>
        /// <param name="userScheduleId">The user schedule identifier.</param>
        /// <param name="applicantId">The applicant identifier.</param>
        /// <param name="meetingType">Type of the meeting.</param>
        public void SendScheduleToApplicant(UserSchedule userSchedule, int userScheduleId, string meetingType, int applicantId = -1)
        {
            // time difference between now and 12hrs before the schedule
            TimeSpan timeDifference = userSchedule.Schedule.AddHours(-12) - DateTime.Now;
            // number of hours left, to be used as the token expiry
            int hoursLeft = (int)timeDifference.TotalHours;

            BackgroundJob.Schedule(() => CheckScheduleStatusAfterTokenExpiry(userScheduleId), TimeSpan.FromHours(hoursLeft));

            Applicant applicant = new Applicant();
            if (applicantId == -1)  // existing schedule
                applicant = _applicantService.GetApplicantByApplicationId(userSchedule.ApplicationId);
            else    // new schedule
                applicant = _applicantService.GetApplicantById(applicantId);
 
            var applicantTemp = _mapper.Map<Applicant>(applicant);
            var userScheduleTemp = _mapper.Map<UserSchedule>(userSchedule);
            BackgroundJob.Enqueue(() => _emailSendingService.SendScheduleToApplicant(userScheduleTemp, applicantTemp, hoursLeft));
        }

        /// <summary>
        /// Checks the schedule status after token expiry.
        /// </summary>
        /// <param name="userScheduleId">The user schedule identifier.</param>
        public void CheckScheduleStatusAfterTokenExpiry(int userScheduleId)
        {
            UserSchedule userSchedule = _userScheduleService.GetUserScheduleById(userScheduleId);
            if (userSchedule.Status == "pending")
            {
                LogContent logContent = RejectSchedule(userScheduleId);
                if (!logContent.Result)
                    _logger.Trace($"Token has expired and was not used. UserSchedule [{userSchedule.Id}] has been automatically rejected.");
            }
        }

        /// <summary>
        /// Sends the schedules to interviewer.
        /// </summary>
        /// <param name="formData">The form data.</param>
        /// <param name="userId">The user identifier.</param>
        public void SendSchedulesToInterviewer(SchedulerDataViewModel formData, int userId)
        {
            string jobOpeningTitle = _jobOpeningService.GetJobOpeningTitleById(formData.JobOpeningId);
            var user = _userService.GetById(userId);
            string scheduledTimes = "";

            foreach (var schedule in formData.ApplicantSchedules)
            {
                var applicant = _applicantService.GetApplicantById(schedule.ApplicantId);
                scheduledTimes += $"{applicant.Firstname} {applicant.Lastname}'s Schedule: {schedule.Time}<br/>";
            }

            BackgroundJob.Enqueue(() => _emailSendingService.SendSchedulesToInterviewer(user.Fullname, user.Email, jobOpeningTitle, formData.Date, scheduledTimes, formData.Type));
        }

        /// <summary>
        /// Accepts the schedule.
        /// </summary>
        /// <param name="userScheduleId">The user schedule identifier.</param>
        public async Task<LogContent> AcceptSchedule(int userScheduleId)
        {
            var userSchedule = _userScheduleService.GetUserScheduleById(userScheduleId);
            LogContent logContent = CheckUserScheduleStatus(userSchedule);

            if (!logContent.Result)
            {
                string joinUrl = await SetOnlineMeetingSchedule(userSchedule);
                if (!string.IsNullOrEmpty(joinUrl))
                {
                    _logger.Trace("Successfully generated a Teams meeting link.");

                    SendAcceptedScheduleToInterviewer(userSchedule, joinUrl);
                    SendAcceptedScheduleToApplicant(userSchedule, joinUrl);

                    _userScheduleService.DeleteUserSchedule(userSchedule);
                }

                string scheduleType = userSchedule.Type.Split(' ').Skip(1).FirstOrDefault();    // Remove first word from string
                if (scheduleType == "Interview")
                {
                    var data = _interviewService.AddInterview(userSchedule, joinUrl);
                    if (!data.Result) _logger.Trace("Successfully created a new Interview record.");
                    else _logger.Error(SetLog(data));
                }
                else    // schedule type is "Exam"
                {
                    var data = _examinationService.AddExamination(userSchedule, joinUrl);
                    if (!data.Result) _logger.Trace("Successfully created a new Examination record.");
                    else _logger.Error(SetLog(data));
                }
            }

            return logContent;
        }

        /// <summary>
        /// Rejects the schedule.
        /// </summary>
        /// <param name="userScheduleId">The user schedule identifier.</param>
        public LogContent RejectSchedule(int userScheduleId)
        {
            var userSchedule = _userScheduleService.GetUserScheduleById(userScheduleId);
            LogContent logContent = CheckUserScheduleStatus(userSchedule);

            if (!logContent.Result)
            {
                userSchedule.Status = "rejected";
                logContent = _userScheduleService.UpdateUserSchedule(userSchedule);
                SendRejectedScheduleNoticeToInterviewer(userSchedule);
            }

            return logContent;
        }

        /// <summary>
        /// Informs the interviewer that a schedule has been rejected.
        /// </summary>
        /// <param name="userSchedule">The user schedule.</param>
        public void SendRejectedScheduleNoticeToInterviewer(UserSchedule userSchedule)
        {
            User user = _userService.GetById(userSchedule.UserId);
            Applicant applicant = _applicantService.GetApplicantByApplicationId(userSchedule.ApplicationId);
            string applicantFullName = applicant.Firstname + " " + applicant.Lastname;

            var userScheduleTemp = _mapper.Map<UserSchedule>(userSchedule);
            BackgroundJob.Enqueue(() => _emailSendingService.SendRejectedScheduleNoticeToInterviewer(user.Email, user.Fullname, userScheduleTemp, applicantFullName));
        }

        /// <summary>
        /// Sets the online meeting schedule.
        /// </summary>
        public async Task<string> SetOnlineMeetingSchedule(UserSchedule userSchedule)
        {
            ApplicationViewModel application = _applicationService.GetById(userSchedule.ApplicationId);
            string email = _userService.GetUserEmailById(userSchedule.UserId);

            CalendarEvent calendarEvent = new CalendarEvent()
            {
                Subject = $"Alliance Software Inc. {userSchedule.Type}",
                Body = new Body()
                {
                    Content = $"{userSchedule.Type} for {application.JobOpeningTitle} position" +
                    $"<br> Applicant Name: {application.ApplicantName}",
                },
                Start = new EventDateTime() { DateTime = userSchedule.Schedule },
                End = new EventDateTime() { DateTime = userSchedule.Schedule.AddHours(1) }
            };

            LogContent logContent = CheckCalendarEvent(calendarEvent);
            if (!logContent.Result)
                return await _calendarService.CreateEvent(calendarEvent, email);

            return "";
        }

        /// <summary>
        /// Sends the accepted schedule with Teams link to the interviewer.
        /// </summary>
        public void SendAcceptedScheduleToInterviewer(UserSchedule userSchedule, string joinUrl)
        {
            User user = _userService.GetById(userSchedule.UserId);
            ApplicationViewModel application = _applicationService.GetById(userSchedule.ApplicationId);

            var userScheduleTemp = _mapper.Map<UserSchedule>(userSchedule);
            BackgroundJob.Enqueue(() => _emailSendingService.SendAcceptedScheduleToInterviewer(user.Email, user.Fullname, userScheduleTemp, application, joinUrl));
        }

        /// <summary>
        /// Sends the accepted schedule with Teams link to the applicant.
        /// </summary>
        public void SendAcceptedScheduleToApplicant(UserSchedule userSchedule, string joinUrl)
        {
            string email = _applicantService.GetApplicantByApplicationId(userSchedule.ApplicationId).Email;
            ApplicationViewModel application = _applicationService.GetById(userSchedule.ApplicationId);

            var userScheduleTemp = _mapper.Map<UserSchedule>(userSchedule);
            BackgroundJob.Enqueue(() => _emailSendingService.SendAcceptedScheduleToApplicant(email, userScheduleTemp, application, joinUrl));
        }
    }
}
