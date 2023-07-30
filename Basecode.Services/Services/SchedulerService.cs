﻿using Basecode.Data.Dto;
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
        
        public SchedulerService(IUserScheduleService userScheduleService, IApplicationService applicationService, IEmailSendingService emailSendingService, IApplicantService applicantService, 
            IUserService userService, ICalendarService calendarService, IInterviewService interviewService, IExaminationService examinationService, IJobOpeningService jobOpeningService)
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
        }

        /// <summary>
        /// Adds the schedules from the HR Scheduler.
        /// </summary>
        /// <param name="formData">The HR Scheduler form data.</param>
        public async Task<(LogContent, Dictionary<string, string>)> AddSchedules(SchedulerDataViewModel formData, int userId)
        {
            (LogContent logContent, Dictionary<string, string> validationErrors) data = CheckSchedulerData(formData);

            if (data.logContent.Result == false)
            {
                List<int> successfullyAddedApplicantIds = new List<int>();

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
                    if (existingId != -1)   // Update "rejected" schedule to "pending"
                    {
                        successfullyAddedApplicantIds = await HandleExistingSchedule(userSchedule, existingId, schedule.ApplicantId, successfullyAddedApplicantIds);
                    }
                    else    // Create new schedule
                    {
                        successfullyAddedApplicantIds = await HandleNewSchedule(userSchedule, schedule.ApplicantId, successfullyAddedApplicantIds);
                    }
                }

                await SendSchedulesToInterviewer(formData, userId, successfullyAddedApplicantIds);
            }

            return data;
        }

        /// <summary>
        /// Handles the existing schedule.
        /// </summary>
        private async Task<List<int>> HandleExistingSchedule(UserSchedule userSchedule, int existingId, int applicantId, List<int> successfullyAddedApplicantIds)
        {
            LogContent logContent = _userScheduleService.UpdateUserSchedule(userSchedule, existingId);
            if (logContent.Result == false)
            {
                _logger.Trace("Successfully updated User Schedule [ " + existingId + " ]");
                await SendScheduleToApplicant(userSchedule, existingId, applicantId, userSchedule.Type);
                successfullyAddedApplicantIds.Add(applicantId);
            }
            return successfullyAddedApplicantIds;
        }

        /// <summary>
        /// Handles the new schedule.
        /// </summary>
        private async Task<List<int>> HandleNewSchedule(UserSchedule userSchedule, int applicantId, List<int> successfullyAddedApplicantIds)
        {
            (LogContent logContent, int userScheduleId) data = _userScheduleService.AddUserSchedule(userSchedule);

            if (!data.logContent.Result && data.userScheduleId != -1)
            {
                _logger.Trace("Successfully created a new UserSchedule record.");
                await SendScheduleToApplicant(userSchedule, data.userScheduleId, applicantId, userSchedule.Type);
                successfullyAddedApplicantIds.Add(applicantId);
            }
            return successfullyAddedApplicantIds;
        }

        /// <summary>
        /// Sends the schedule to applicant.
        /// </summary>
        /// <param name="userSchedule">The user schedule.</param>
        /// <param name="userScheduleId">The user schedule identifier.</param>
        /// <param name="applicantId">The applicant identifier.</param>
        /// <param name="meetingType">Type of the meeting.</param>
        public async Task SendScheduleToApplicant(UserSchedule userSchedule, int userScheduleId, int applicantId, string meetingType)
        {
            var applicant = _applicantService.GetApplicantById(applicantId);

            // time difference between now and 12hrs before the schedule
            TimeSpan timeDifference = userSchedule.Schedule.AddHours(-12) - DateTime.Now;
            // number of hours left, to be used as the token expiry
            int hoursLeft = (int)timeDifference.TotalHours;

            BackgroundJob.Schedule(() => CheckScheduleStatusAfterTokenExpiry(userScheduleId), TimeSpan.FromHours(hoursLeft));
            await _emailSendingService.SendScheduleToApplicant(userSchedule, userScheduleId, applicant, meetingType, hoursLeft);
        }


        private void CheckScheduleStatusAfterTokenExpiry(int userScheduleId)
        {
            UserSchedule userSchedule = _userScheduleService.GetUserScheduleById(userScheduleId);
            if (userSchedule.Status == "pending")
            {
                LogContent logContent = RejectSchedule(userScheduleId);
                if (!logContent.Result)
                {
                    _logger.Trace($"Token has expired and was not used. UserSchedule [ {userSchedule.Id} ] has been automatically rejected.");
                }
            }
        }

        /// <summary>
        /// Sends the schedules to interviewer.
        /// </summary>
        /// <param name="formData">The form data.</param>
        /// <param name="userId">The user identifier.</param>
        public async Task SendSchedulesToInterviewer(SchedulerDataViewModel formData, int userId, List<int> successfullyAddedApplicantIds)
        {
            string jobOpeningTitle = _jobOpeningService.GetJobOpeningTitleById(formData.JobOpeningId);
            var user = _userService.GetById(userId);
            string scheduledTimes = "";

            foreach (var schedule in formData.ApplicantSchedules)
            {
                if (successfullyAddedApplicantIds.Contains(schedule.ApplicantId))
                {
                    var applicant = _applicantService.GetApplicantById(schedule.ApplicantId);
                    scheduledTimes += $"{applicant.Firstname} {applicant.Lastname}'s Schedule: {schedule.Time}<br/>";
                }
            }

            await _emailSendingService.SendSchedulesToInterviewer(user.Fullname, user.Email, jobOpeningTitle, formData.Date, scheduledTimes, formData.Type);
        }

        /// <summary>
        /// Accepts the schedule.
        /// </summary>
        /// <param name="userScheduleId">The user schedule identifier.</param>
        public async Task<LogContent> AcceptSchedule(int userScheduleId)
        {
            var userSchedule = _userScheduleService.GetUserScheduleById(userScheduleId);
            LogContent logContent = CheckUserScheduleStatus(userSchedule);

            if (logContent.Result == false)
            {
                LogContent data;
                string scheduleType = userSchedule.Type.Split(' ').Skip(1).FirstOrDefault();
                if (scheduleType == "Interview")
                {
                    data = _interviewService.AddInterview(userSchedule);
                    if (!data.Result)
                    {
                        _logger.Trace("Successfully created a new Interview record.");
                    }
                }
                else    // schedule type is Exam
                {
                    data = _examinationService.AddExamination(userSchedule);
                    if (!data.Result)
                    {
                        _logger.Trace("Successfully created a new Examination record.");
                    }
                }

                if (data.Result == false)
                {
                    string joinUrl = SetOnlineMeetingSchedule(userSchedule);
                    if (!string.IsNullOrEmpty(joinUrl))
                    {
                        _logger.Trace("Successfully generated a Teams meeting link.");

                        await SendAcceptedScheduleToInterviewer(userSchedule, joinUrl);
                        await SendAcceptedScheduleToApplicant(userSchedule, joinUrl);

                        _userScheduleService.DeleteUserSchedule(userSchedule);
                    }
                }
            }

            return logContent;
        }

        /// <summary>
        /// Rejects the schedule.
        /// </summary>
        /// <param name="userScheduleId">The user schedule identifier.</param>
        public async Task<LogContent> RejectSchedule(int userScheduleId)
        {
            var userSchedule = _userScheduleService.GetUserScheduleById(userScheduleId);
            LogContent logContent = CheckUserScheduleStatus(userSchedule);

            if (logContent.Result == false)
            {
                userSchedule.Status = "rejected";
                logContent = _userScheduleService.UpdateUserSchedule(userSchedule);
                await SendRejectedScheduleNoticeToInterviewer(userSchedule);
            }

            return logContent;
        }

        /// <summary>
        /// Informs the interviewer that a schedule has been rejected.
        /// </summary>
        /// <param name="userSchedule">The user schedule.</param>
        public async Task SendRejectedScheduleNoticeToInterviewer(UserSchedule userSchedule)
        {
            User user = _userService.GetById(userSchedule.UserId);
            Applicant applicant = _applicantService.GetApplicantByApplicationId(userSchedule.ApplicationId);
            string applicantFullName = applicant.Firstname + " " + applicant.Lastname;
            await _emailSendingService.SendRejectedScheduleNoticeToInterviewer(user.Email, user.Fullname, userSchedule, applicantFullName);
        }

        /// <summary>
        /// Sets the online meeting schedule.
        /// </summary>
        public string SetOnlineMeetingSchedule(UserSchedule userSchedule)
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

            string joinUrl = "";
            LogContent logContent = CheckCalendarEvent(calendarEvent);
            if (logContent.Result == false)
            {
                joinUrl = _calendarService.CreateEvent(calendarEvent, email);
            }

            return joinUrl;
        }

        /// <summary>
        /// Sends the accepted schedule with Teams link to the interviewer.
        /// </summary>
        public async Task SendAcceptedScheduleToInterviewer(UserSchedule userSchedule, string joinUrl)
        {
            User user = _userService.GetById(userSchedule.UserId);
            ApplicationViewModel application = _applicationService.GetById(userSchedule.ApplicationId);
            await _emailSendingService.SendAcceptedScheduleToInterviewer(user.Email, user.Fullname, userSchedule, application, joinUrl);
        }

        /// <summary>
        /// Sends the accepted schedule with Teams link to the applicant.
        /// </summary>
        public async Task SendAcceptedScheduleToApplicant(UserSchedule userSchedule, string joinUrl)
        {
            string email = _applicantService.GetApplicantByApplicationId(userSchedule.ApplicationId).Email;
            ApplicationViewModel application = _applicationService.GetById(userSchedule.ApplicationId);
            await _emailSendingService.SendAcceptedScheduleToApplicant(email, userSchedule, application, joinUrl);
        }
    }
}
