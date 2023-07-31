using NLog;
using static Basecode.Services.Services.ErrorHandling;
using Basecode.Data.Interfaces;
using Basecode.Data.Models;
using Basecode.Data.ViewModels;
using Basecode.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Basecode.Data.Repositories;

namespace Basecode.Services.Services
{
    public class CurrenHireService : ErrorHandling, ICurrentHireService
    {
        private readonly ICurrentHireRepository _currentHireRepository;
        private readonly IUserScheduleRepository _userScheduleRepository;
        private readonly IApplicationRepository _applicationRepository;
        private readonly IApplicantService _applicantService;
        private readonly IUserService _userService;
        private readonly IEmailSendingService _emailSendingService;

        public CurrenHireService(ICurrentHireRepository currentHireRepository, IUserScheduleRepository userScheduleRepository, IApplicationRepository applicationRepository, IApplicantService applicantService, IUserService userService, 
            IEmailSendingService emailSendingService)
        {
            _currentHireRepository = currentHireRepository;
            _userScheduleRepository = userScheduleRepository;
            _applicationRepository = applicationRepository;
            _applicantService = applicantService;
            _userService = userService;
            _emailSendingService = emailSendingService;
        }

        /// <summary>
        /// Get UserOffer by Id
        /// </summary>
        /// <param name="userOfferId"></param>
        /// <returns></returns>
        public CurrentHire GetCurrentHireById(int currenHireId)
        {
            return _currentHireRepository.GetCurrentHireById(currenHireId);
        }

        /// <summary>
        /// AcceptOffer and return UserOffer Status
        /// </summary>
        /// <param name="userOfferId"></param>
        /// <returns></returns>
        public LogContent AcceptOffer(int currentHireId)
        {
            var currentHire = GetCurrentHireById(currentHireId);
            LogContent logContent = CheckCurrentHireStatus(currentHire);

            if (logContent.Result == false)
            {
                currentHire.Status = "confirm";
                logContent = UpdateCurrentHire(currentHire);
            }

            return logContent;
        }

        /// <summary>
        /// Get UserSchedule by Id
        /// </summary>
        /// <param name="userScheduleId"></param>
        /// <returns></returns>
        public UserSchedule GetUserScheduleById(int userScheduleId)
        {
            return _userScheduleRepository.GetUserScheduleById(userScheduleId);
        }

        /// <summary>
        /// Reject offer and return UserOffer Status
        /// </summary>
        /// <param name="userOfferId"></param>
        /// <returns></returns>
        public async Task<LogContent> RejectOffer(int currentHireId)
        {
            var userSchedule = GetUserScheduleById(currentHireId);
            var application = _applicationRepository.GetById(userSchedule.ApplicationId);
            var applicant = _applicantService.GetApplicantById(application.ApplicantId);
            await AddCurrentHire(applicant, currentHireId);

            var hireId = _currentHireRepository.GetCurrentHireIdByUserId(currentHireId);
            var currentHire = _currentHireRepository.GetCurrentHireById(hireId);

            LogContent logContent = CheckCurrentHireStatus(currentHire);
            if (logContent.Result == false)
            {
                currentHire.Status = "rejected";
                logContent = UpdateCurrentHire(currentHire);
                await SendRejectedHireNoticeToInterviewer(currentHire);
            }
            return logContent;
        }

        /// <summary>
        /// Send reject notice to interviewer
        /// </summary>
        /// <param name="userOffer"></param>
        /// <returns></returns>
        public async Task SendRejectedHireNoticeToInterviewer(CurrentHire currentHire)
        {
            User user = _userService.GetById(currentHire.UserId);
            Applicant applicant = _applicantService.GetApplicantByApplicationId(currentHire.ApplicationId);
            string applicantFullName = applicant.Firstname + " " + applicant.Lastname;
            await _emailSendingService.SendRejectedHireNoticeToInterviewer(user.Email, user.Fullname, applicant.Application.JobOpening.Title, currentHire, applicantFullName);
        }
    }
}
