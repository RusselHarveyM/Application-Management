using Basecode.Data.Dto;
using Basecode.Data.Interfaces;
using Basecode.Data.Models;
using Basecode.Data.ViewModels;
using Basecode.Services.Interfaces;
using NLog;

namespace Basecode.Services.Services
{
    public class UserScheduleService : ErrorHandling, IUserScheduleService
    {
        private readonly IUserScheduleRepository _repository;
        private readonly IEmailSendingService _emailSendingService;
        private readonly IApplicationService _applicationService;
        private readonly IApplicantService _applicantService;
        private readonly IUserService _userService;
        private readonly IJobOpeningService _jobOpeningService;
        private readonly IInterviewService _interviewService;
        private readonly IExaminationService _examinationService;
        private readonly ICalendarService _calendarService;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public UserScheduleService(IUserScheduleRepository repository, IEmailSendingService emailSendingService, IApplicationService applicationService, IApplicantService applicantService,
            IUserService userService, IJobOpeningService jobOpeningService, IInterviewService interviewService, IExaminationService examinationService, ICalendarService calendarService)
        {
            _repository = repository;
            _emailSendingService = emailSendingService;
            _applicationService = applicationService;
            _applicantService = applicantService;
            _userService = userService;
            _jobOpeningService = jobOpeningService;
            _interviewService = interviewService;
            _examinationService = examinationService;
            _calendarService = calendarService;
        }

        /// <summary>
        /// Adds the schedules from the HR Scheduler.
        /// </summary>
        /// <param name="formData">The HR Scheduler form data.</param>
        public async Task<(LogContent, Dictionary<string, string>)> AddUserSchedules(SchedulerDataViewModel formData, int userId)
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

                    int existingId = GetIdIfUserScheduleExists(applicationId);
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
            LogContent logContent = UpdateUserSchedule(userSchedule, existingId);
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
            (LogContent logContent, int userScheduleId) data = AddUserSchedule(userSchedule);

            if (!data.logContent.Result && data.userScheduleId != -1)
            {
                _logger.Trace("Successfully created a new UserSchedule record.");
                await SendScheduleToApplicant(userSchedule, data.userScheduleId, applicantId, userSchedule.Type);
                successfullyAddedApplicantIds.Add(applicantId);
            }
            return successfullyAddedApplicantIds;
        }

        /// <summary>
        /// Gets the identifier if user schedule exists.
        /// </summary>
        /// <param name="applicationId">The application identifier.</param>
        /// <returns>The user schedule identifier.</returns>
        public int GetIdIfUserScheduleExists(Guid applicationId)
        {
            return _repository.GetIdIfUserScheduleExists(applicationId);
        }

        /// <summary>
        /// Adds a schedule in the UserSchedule table.
        /// </summary>
        /// <param name="userSchedule">The schedule.</param>
        /// <returns></returns>
        public (LogContent, int) AddUserSchedule(UserSchedule userSchedule)
        {
            LogContent logContent = CheckUserSchedule(userSchedule);
            int userScheduleId = -1;

            if (logContent.Result == false)
            {
                userScheduleId = _repository.AddUserSchedule(userSchedule);
            }

            return (logContent, userScheduleId);
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
            await _emailSendingService.SendScheduleToApplicant(userSchedule, userScheduleId, applicant, meetingType);
        }

        /// <summary>
        /// Gets the user schedule by identifier.
        /// </summary>
        /// <param name="userScheduleId">The user schedule identifier.</param>
        /// <returns></returns>
        public UserSchedule GetUserScheduleById(int userScheduleId)
        {
            return _repository.GetUserScheduleById(userScheduleId);
        }

        /// <summary>
        /// Updates the schedule.
        /// </summary>
        public LogContent UpdateUserSchedule(UserSchedule userSchedule, int? idToSetAsPending = null)
        {
            LogContent logContent = CheckUserSchedule(userSchedule);

            if (logContent.Result == false)
            {
                int idToUpdate = idToSetAsPending ?? userSchedule.Id;

                var scheduleToBeUpdated = _repository.GetUserScheduleById(idToUpdate);
                scheduleToBeUpdated.Schedule = userSchedule.Schedule;
                scheduleToBeUpdated.Status = userSchedule.Status;
                _repository.UpdateUserSchedule(scheduleToBeUpdated);
            }

            return logContent;
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
        /// <returns></returns>
        public async Task<LogContent> AcceptSchedule(int userScheduleId)
        {
            var userSchedule = GetUserScheduleById(userScheduleId);
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

                        DeleteUserSchedule(userSchedule);
                    }
                } 
            }

            return logContent;
        }

        /// <summary>
        /// Rejects the schedule.
        /// </summary>
        /// <param name="userScheduleId">The user schedule identifier.</param>
        /// <returns></returns>
        public async Task<LogContent> RejectSchedule(int userScheduleId)
        {
            var userSchedule = GetUserScheduleById(userScheduleId);
            LogContent logContent = CheckUserScheduleStatus(userSchedule);

            if (logContent.Result == false)
            {
                userSchedule.Status = "rejected";
                logContent = UpdateUserSchedule(userSchedule);
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

        /// <summary>
        /// Deletes the user schedule.
        /// </summary>
        /// <param name="userSchedule">The user schedule.</param>
        public void DeleteUserSchedule(UserSchedule userSchedule)
        {
            _repository.DeleteUserSchedule(userSchedule);
            _logger.Trace("UserSchedule record has been deleted.");
        }
    }
}
