using Basecode.Data.Interfaces;
using Basecode.Data.Models;
using Basecode.Services.Interfaces;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace Basecode.Services.Services;

public class CurrentHireService : ErrorHandling, ICurrentHireService
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly IApplicantService _applicantService;
    private readonly IApplicationService _applicationService;
    private readonly ICurrentHireRepository _currentHireRepository;
    private readonly IUserScheduleRepository _userScheduleRepository;
    private readonly IScheduleSendingService _scheduleSendingService;
    private readonly ITrackService _trackService;
    private readonly IUserService _userService;

    public CurrentHireService(ICurrentHireRepository currentHireRepository, IUserScheduleRepository userScheduleRepository, IApplicantService applicantService, IApplicationService applicationService, 
        IScheduleSendingService scheduleSendingService, ITrackService trackService, IUserService userService)
    {
        _currentHireRepository = currentHireRepository;
        _userScheduleRepository = userScheduleRepository;
        _applicantService = applicantService;
        _applicationService = applicationService;
        _scheduleSendingService = scheduleSendingService;
        _trackService = trackService;
        _userService = userService;
    }

    /// <summary>
    ///     Get UserOffer by Id
    /// </summary>
    /// <param name="userOfferId"></param>
    /// <returns></returns>
    public CurrentHire GetCurrentHireById(int currenHireId)
    {
        return _currentHireRepository.GetCurrentHireById(currenHireId);
    }

    /// <summary>
    ///     AcceptOffer and return UserOffer Status
    /// </summary>
    /// <param name="userOfferId"></param>
    /// <returns></returns>
    public LogContent AcceptOffer(int currentHireId)
    {
        var currentHire = GetCurrentHireById(currentHireId);
        var logContent = CheckCurrentHireStatus(currentHire);

        if (logContent.Result == false)
        {
            currentHire.Status = "confirm";
            logContent = UpdateCurrentHire(currentHire);
        }

        return logContent;
    }

    /// <summary>
    ///     Get UserSchedule by Id
    /// </summary>
    /// <param name="userScheduleId"></param>
    /// <returns></returns>
    public UserSchedule GetUserScheduleById(int userScheduleId)
    {
        return _userScheduleRepository.GetUserScheduleById(userScheduleId);
    }

    /// <summary>
    ///     update user offer
    /// </summary>
    /// <param name="userOffer"></param>
    /// <param name="idToSetAsPending"></param>
    /// <returns></returns>
    public LogContent UpdateCurrentHire(CurrentHire currentHire, int? idToSetAsPending = null)
    {
        var applicant = _applicantService.GetApplicantByApplicationId(currentHire.ApplicationId);
        var application = _applicationService.GetApplicationByApplicantId(applicant.Id);
        var userschedule = _userScheduleRepository.GetApplicationByGuid(application.Id);
        var logContent = CheckCurrentHire(currentHire);

        if (logContent.Result == false)
        {
            var idToUpdate = idToSetAsPending ?? currentHire.Id;

            var hireToBeUpdated = _currentHireRepository.GetCurrentHireById(idToUpdate);
            //offerToBeUpdated.Offer = userOffer.Offer;
            hireToBeUpdated.Firstname = applicant.Firstname;
            hireToBeUpdated.Middlename = applicant.Middlename;
            hireToBeUpdated.Lastname = applicant.Lastname;
            hireToBeUpdated.Phone = applicant.Phone;
            hireToBeUpdated.Email = applicant.Email;
            hireToBeUpdated.Status = "Confirmed";
            _currentHireRepository.UpdateCurrentHire(hireToBeUpdated);

            if (hireToBeUpdated.Status == "Confirmed")
            {
                _scheduleSendingService.SendDeploymentApprovalEmail(userschedule);
            }
            
        }

        return logContent;
    }

    /// <summary>
    ///     Get Id if user offer existed
    /// </summary>
    /// <param name="applicationId"></param>
    /// <returns></returns>
    public int GetIdIfCurrentHireExists(Guid applicationId)
    {
        return _currentHireRepository.GetIdIfCurrentHireExists(applicationId);
    }

    /// <summary>
    ///     Add useroffer
    /// </summary>
    /// <param name="applicant"></param>
    /// <param name="userOfferId"></param>
    /// <returns></returns>
    public async Task AddCurrentHire(Applicant applicant, int currentHireId)
    {
        var successfullyAddedApplicantIds = new List<int>();
        var userSchedule = GetUserScheduleById(currentHireId);
        var applicationId = _applicationService.GetApplicationIdByApplicantId(applicant.Id);

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
            Status = "To Be Confirmed",
            User = userSchedule.User,
            Application = userSchedule.Application
        };

        var existingId = GetIdIfCurrentHireExists(applicationId);
        if (existingId != -1)
            successfullyAddedApplicantIds =
                await HandleExistingHire(currentHire, existingId, applicant.Id, successfullyAddedApplicantIds);
        else 
            successfullyAddedApplicantIds =
                await HandleNewHire(currentHire, applicant.Id, successfullyAddedApplicantIds);
    }

    /// <summary>
    ///     Handle existing offer
    /// </summary>
    /// <param name="currentHire"></param>
    /// <param name="existingId"></param>
    /// <param name="applicantId"></param>
    /// <param name="successfullyAddedApplicantIds"></param>
    /// <returns></returns>
    public async Task<List<int>> HandleExistingHire(CurrentHire currentHire, int existingId, int applicantId,
        List<int> successfullyAddedApplicantIds)
    {
        var logContent = UpdateCurrentHire(currentHire, existingId);
        if (logContent.Result == false)
        {
            _logger.Trace("Successfully updated User Offer [ " + existingId + " ]");
            // await SendScheduleToApplicant(userOffer, existingId, applicantId);
            successfullyAddedApplicantIds.Add(applicantId);
        }

        return successfullyAddedApplicantIds;
    }

    /// <summary>
    ///     Handle new offer
    /// </summary>
    /// <param name="userOffer"></param>
    /// <param name="applicantId"></param>
    /// <param name="successfullyAddedApplicantIds"></param>
    /// <returns></returns>
    public async Task<List<int>> HandleNewHire(CurrentHire currentHire, int applicantId,
        List<int> successfullyAddedApplicantIds)
    {
        (LogContent logContent, int currentHireId) data = AddCurrentHires(currentHire);
        var application = _applicationService.GetApplicationByApplicantId(applicantId);
        var userschedule = _userScheduleRepository.GetApplicationByGuid(application.Id);

        if (!data.logContent.Result && data.currentHireId != -1)
        {
            _logger.Trace("Successfully added New Hire.");
            //Send Confirmation to Email to DT
            //await SendHireToApplicant(currentHire, data.currentHireId, applicantId);
            successfullyAddedApplicantIds.Add(applicantId);
            _scheduleSendingService.SendNotifyToDT(applicantId);
            _scheduleSendingService.ScheduleConfirmationEmailToDT(userschedule);
        }

        return successfullyAddedApplicantIds;
    }

    /// <summary>
    ///     Add useroffers
    /// </summary>
    /// <param name="currentHire"></param>
    /// <returns></returns>
    public (LogContent, int) AddCurrentHires(CurrentHire currentHire)
    {
        var logContent = CheckCurrentHire(currentHire);
        var currentHireId = -1;

        if (logContent.Result == false) currentHireId = _currentHireRepository.AddCurrentHire(currentHire);

        return (logContent, currentHireId);
    }

    public List<CurrentHire> GetShortListedCurrentHire(string stage)
    {
        var data = _currentHireRepository.GetAll()
            .Where(m => m.Status == stage)
            .ToList();
        return data;
    }

    public int CurrentHireAcceptOffer(Dictionary<string, string> tokenClaims)
    {
        if (tokenClaims.Count == 0)
        {
            _logger.Warn("Invalid or expired token.");
            return -1;
        }

        var userId = int.Parse(tokenClaims["userId"]);
        var appId = Guid.Parse(tokenClaims["appId"]);
        var status = tokenClaims["newStatus"];
        var choice = tokenClaims["choice"];

        //Check if applicant and user exists
        var application = _applicationService.GetApplicationById(appId);
        var user = _userService.GetById(userId);

        if (application != null && user != null)
        {
            var result = _trackService.UpdateApplicationStatusByEmailResponseCurrentHires(application, user, choice, status);
            _applicationService.Update(result);
            var userSchedule = _userScheduleRepository.GetApplicationByGuid(appId);
            var applicant = _applicantService.GetApplicantByApplicationId(application.Id);
            if (result.Status == "Undergoing Job Offer")
            {
                _scheduleSendingService.SendJobOfferEmailToApplicant(userSchedule);
            }
            else if (result.Status == "Confirmed")
            {
                AddCurrentHire(applicant, userSchedule.Id);
            }
            else if (result.Status == "Onboarding")
            {
                AddCurrentHire(applicant, userSchedule.Id);
            }
            else if (result.Status == "Deployed")
            {
                _scheduleSendingService.SendCongratulationEmailToApplicant(userSchedule);
            }
        }

        return 1;
    }
}
