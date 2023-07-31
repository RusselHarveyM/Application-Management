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
        private readonly IApplicationService _applicationService;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public CurrenHireService(ICurrentHireRepository currentHireRepository, IUserScheduleRepository userScheduleRepository, IApplicationRepository applicationRepository, IApplicantService applicantService, IUserService userService, 
            IEmailSendingService emailSendingService, IApplicationService applicationService)
        {
            _currentHireRepository = currentHireRepository;
            _userScheduleRepository = userScheduleRepository;
            _applicationRepository = applicationRepository;
            _applicantService = applicantService;
            _userService = userService;
            _emailSendingService = emailSendingService;
            _applicationService = applicationService;
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

        /// <summary>
        /// update user offer
        /// </summary>
        /// <param name="userOffer"></param>
        /// <param name="idToSetAsPending"></param>
        /// <returns></returns>
        public LogContent UpdateCurrentHire(CurrentHire currentHire, int? idToSetAsPending = null)
        {
            Applicant applicant = _applicantService.GetApplicantByApplicationId(currentHire.ApplicationId);
            LogContent logContent = CheckCurrentHire(currentHire);

            if (logContent.Result == false)
            {
                int idToUpdate = idToSetAsPending ?? currentHire.Id;

                var hireToBeUpdated = _currentHireRepository.GetCurrentHireById(idToUpdate);
                //offerToBeUpdated.Offer = userOffer.Offer;
                hireToBeUpdated.Firstname = applicant.Firstname;
                hireToBeUpdated.Middlename = applicant.Middlename;
                hireToBeUpdated.Lastname = applicant.Lastname;
                hireToBeUpdated.Phone = applicant.Phone;
                hireToBeUpdated.Email = applicant.Email;
                hireToBeUpdated.Status = currentHire.Status;
                _currentHireRepository.UpdateCurrentHire(hireToBeUpdated);
            }

            return logContent;
        }

        /// <summary>
        /// Get Id if user offer existed
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public int GetIdIfCurrentHireExists(Guid applicationId)
        {
            return _currentHireRepository.GetIdIfCurrentHireExists(applicationId);
        }

        /// <summary>
        /// Add useroffer
        /// </summary>
        /// <param name="applicant"></param>
        /// <param name="userOfferId"></param>
        /// <returns></returns>
        public async Task AddCurrenHire(Applicant applicant, int currentHireId)
        {
            List<int> successfullyAddedApplicantIds = new List<int>();
            var userSchedule = GetUserScheduleById(currentHireId);
            Guid applicationId = _applicationService.GetApplicationIdByApplicantId(applicant.Id);

            var currentHire = new CurrentHire
            {
                // Id = userOfferId,
                UserId = currentHireId,
                ApplicationId = userSchedule.ApplicationId,
                Firstname = applicant.Firstname,
                Middlename = applicant.Middlename,
                Lastname = applicant.Lastname,
                Phone = applicant.Phone,
                Email = applicant.Email,
                Status = "pending",
                User = userSchedule.User,
                Application = userSchedule.Application
            };

            int existingId = GetIdIfCurrentHireExists(applicationId);
            if (existingId != -1)   // Update "rejected" schedule to "pending"
            {
                successfullyAddedApplicantIds = await HandleExistingHire(currentHire, existingId, applicant.Id, successfullyAddedApplicantIds);
            }
            else    // Create new schedule
            {
                successfullyAddedApplicantIds = await HandleNewHire(currentHire, applicant.Id, successfullyAddedApplicantIds);
            }
        }

        /// <summary>
        /// Handle existing offer
        /// </summary>
        /// <param name="userOffer"></param>
        /// <param name="existingId"></param>
        /// <param name="applicantId"></param>
        /// <param name="successfullyAddedApplicantIds"></param>
        /// <returns></returns>
        public async Task<List<int>> HandleExistingHire(CurrentHire currentHire, int existingId, int applicantId, List<int> successfullyAddedApplicantIds)
        {
            LogContent logContent = UpdateCurrentHire(currentHire, existingId);
            if (logContent.Result == false)
            {
                _logger.Trace("Successfully updated User Offer [ " + existingId + " ]");
                // await SendScheduleToApplicant(userOffer, existingId, applicantId);
                successfullyAddedApplicantIds.Add(applicantId);
            }
            return successfullyAddedApplicantIds;
        }

        /// <summary>
        /// Send offer to applicant
        /// </summary>
        /// <param name="userOffer"></param>
        /// <param name="userOfferId"></param>
        /// <param name="applicantId"></param>
        /// <returns></returns>
        public async Task SendHireToApplicant(CurrentHire currentHire, int currentHireId, int applicantId)
        {
            var applicant = _applicantService.GetApplicantById(applicantId);
            //await _emailSendingService.SendScheduleToApplicant(userOffer, userOfferId, applicant);
        }

        /// <summary>
        /// Handle new offer
        /// </summary>
        /// <param name="userOffer"></param>
        /// <param name="applicantId"></param>
        /// <param name="successfullyAddedApplicantIds"></param>
        /// <returns></returns>
        public async Task<List<int>> HandleNewHire(CurrentHire currentHire, int applicantId, List<int> successfullyAddedApplicantIds)
        {
            (LogContent logContent, int currentHireId) data = AddCurrentHires(currentHire);

            if (!data.logContent.Result && data.currentHireId != -1)
            {
                _logger.Trace("Successfully created a new UserSchedule record.");
                await SendHireToApplicant(currentHire, data.currentHireId, applicantId);
                successfullyAddedApplicantIds.Add(applicantId);
            }
            return successfullyAddedApplicantIds;
        }

        /// <summary>
        /// Add useroffers
        /// </summary>
        /// <param name="userOffer"></param>
        /// <returns></returns>
        public (LogContent, int) AddCurrentHires(CurrentHire currentHire)
        {
            LogContent logContent = CheckCurrentHire(currentHire);
            int currentHireId = -1;

            if (logContent.Result == false)
            {
                currentHireId = _currentHireRepository.AddCurrentHire(currentHire);
            }

            return (logContent, currentHireId);
        }
    }
}
