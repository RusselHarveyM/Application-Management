using AutoMapper;
using Basecode.Data.Models;
using Basecode.Data.ViewModels;
using Basecode.Services.Interfaces;
using Hangfire;

namespace Basecode.Services.Services
{
    public class ScheduleSendingService : IScheduleSendingService
    {
        private readonly IEmailSendingService _emailSendingService;
        private readonly IApplicationService _applicationService;
        private readonly IJobOpeningService _jobOpeningService;
        private readonly IApplicantService _applicantService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public ScheduleSendingService(IEmailSendingService emailSendingService, IApplicationService applicationService, IJobOpeningService jobOpeningService, 
            IApplicantService applicantService, IUserService userService, IMapper mapper)
        {
            _emailSendingService = emailSendingService;
            _applicationService = applicationService;
            _jobOpeningService = jobOpeningService;
            _applicantService = applicantService;
            _userService = userService;
            _mapper = mapper;
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
            TimeSpan timeDifference = userSchedule.Schedule.AddHours(-12) - DateTime.Now;
            int tokenExpiry = (int)timeDifference.TotalHours;

            Applicant applicant = new Applicant();
            if (applicantId == -1)  // existing schedule
                applicant = _applicantService.GetApplicantByApplicationId(userSchedule.ApplicationId);
            else    // new schedule
                applicant = _applicantService.GetApplicantById(applicantId);

            var applicantTemp = _mapper.Map<Applicant>(applicant);
            var userScheduleTemp = _mapper.Map<UserSchedule>(userSchedule);
            BackgroundJob.Enqueue(() => _emailSendingService.SendScheduleToApplicant(userScheduleTemp, applicantTemp, tokenExpiry));
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

        /// <summary>
        /// Schedules the sending of the approval email.
        /// </summary>
        /// <param name="userSchedule">The user schedule.</param>
        public void ScheduleSendApprovalEmail(UserSchedule userSchedule)
        {
            User user = _userService.GetById(userSchedule.UserId);
            Applicant applicant = _applicantService.GetApplicantByApplicationId(userSchedule.ApplicationId);
            var userTemp = _mapper.Map<User>(user);
            var applicantTemp = _mapper.Map<Applicant>(applicant);
            string newStatus = "For " + userSchedule.Type;
            int hoursLeft = (int)(userSchedule.Schedule - DateTime.Now).TotalHours;
            BackgroundJob.Schedule(() => _emailSendingService.SendApprovalEmail(userTemp, applicantTemp, userSchedule.ApplicationId, newStatus), TimeSpan.FromHours(hoursLeft));
        }
    }
}
