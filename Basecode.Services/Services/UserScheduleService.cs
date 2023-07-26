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
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public UserScheduleService(IUserScheduleRepository repository, IEmailSendingService emailSendingService, IApplicationService applicationService, IApplicantService applicantService,
            IUserService userService, IJobOpeningService jobOpeningService, IInterviewService interviewService, IExaminationService examinationService)
        {
            _repository = repository;
            _emailSendingService = emailSendingService;
            _applicationService = applicationService;
            _applicantService = applicantService;
            _userService = userService;
            _jobOpeningService = jobOpeningService;
            _interviewService = interviewService;
            _examinationService = examinationService;
        }

        /// <summary>
        /// Adds the schedules from the HR Scheduler.
        /// </summary>
        /// <param name="formData">The HR Scheduler form data.</param>
        public async Task AddUserSchedules(SchedulerDataViewModel formData)
        {
            int userId = 1; // Temporary until auth has been sorted out
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

                (LogContent logContent, int userScheduleId) data = AddUserSchedule(userSchedule);

                if (!data.logContent.Result && data.userScheduleId != -1)
                {
                    _logger.Trace("Successfully created a new UserSchedule record.");
                    await SendScheduleToApplicant(userSchedule, data.userScheduleId, schedule.ApplicantId, formData.Type);
                    successfullyAddedApplicantIds.Add(schedule.ApplicantId);
                }
            }

            await SendSchedulesToInterviewer(formData, userId, successfullyAddedApplicantIds);
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
            meetingType = meetingType[(meetingType.Split()[0].Length + 1)..];   // Remove "For " from meeting type
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
        /// <param name="userSchedule">The user schedule.</param>
        /// <returns></returns>
        public LogContent UpdateUserSchedule(UserSchedule userSchedule)
        {
            LogContent logContent = CheckUserSchedule(userSchedule);

            if (logContent.Result == false)
            {
                var scheduleToBeUpdated = _repository.GetUserScheduleById(userSchedule.Id);
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
            string meetingType = formData.Type[(formData.Type.Split()[0].Length + 1)..];   // Remove "For " from meeting type
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

            await _emailSendingService.SendSchedulesToInterviewer(user.Fullname, user.Email, jobOpeningTitle, formData.Date, scheduledTimes, meetingType);
        }

        /// <summary>
        /// Accepts the schedule.
        /// </summary>
        /// <param name="userScheduleId">The user schedule identifier.</param>
        /// <returns></returns>
        public LogContent AcceptSchedule(int userScheduleId)
        {
            var userSchedule = GetUserScheduleById(userScheduleId);
            LogContent logContent = CheckAcceptedSchedule(userSchedule);

            if (logContent.Result == false)
            {
                if (userSchedule.Type == "For HR Interview" || userSchedule.Type == "For Technical Interview")
                {
                    var interviewData = _interviewService.AddInterview(userSchedule);
                    if (!interviewData.Result)
                    {
                        _logger.Trace("Successfully created a new Interview record.");
                    }
                }
                else
                {
                    var examData = _examinationService.AddExamination(userSchedule);
                    if (!examData.Result)
                    {
                        _logger.Trace("Successfully created a new Examination record.");
                    }
                }

                userSchedule.Status = "accepted";
                logContent = UpdateUserSchedule(userSchedule);
            }

            return logContent;
        }
    }
}
