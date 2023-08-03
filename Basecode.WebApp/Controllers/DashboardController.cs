using Basecode.Data.Models;
using Basecode.Data.ViewModels;
using Basecode.Services.Interfaces;
using Basecode.Services.Services;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NLog;
using NToastNotify;

namespace Basecode.WebApp.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly IApplicantService _applicantService;
    private readonly IBackgroundCheckService _backgroundCheckService;
    private readonly ICharacterReferenceService _characterReferenceService;
    private readonly IDashboardService _dashboardService;
    private readonly IEmailSendingService _emailSendingService;
    private readonly IJobOpeningService _jobOpeningService;
    private readonly IApplicationService _applicationService;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IUserService _userService;
    private readonly IToastNotification _toastNotification;
    private readonly IExaminationService _examinationService;
    private readonly ITrackService _trackService;


    public DashboardController(IApplicantService applicantService, IJobOpeningService jobOpeningService,
        IUserService userService, ICharacterReferenceService characterReferenceService,
        IBackgroundCheckService backgroundCheckService, IEmailSendingService emailSendingService,
        IDashboardService dashboardService, UserManager<IdentityUser> userManager, IToastNotification toastNotification,
        IApplicationService applicationService,
        IExaminationService examinationService, ITrackService trackService)
    {
        _applicantService = applicantService;
        _jobOpeningService = jobOpeningService;
        _userService = userService;
        _characterReferenceService = characterReferenceService;
        _backgroundCheckService = backgroundCheckService;
        _emailSendingService = emailSendingService;
        _dashboardService = dashboardService;
        _userManager = userManager;
        _toastNotification = toastNotification;
        _applicationService = applicationService;
        _examinationService = examinationService;
        _trackService = trackService;
    }

    /// <summary>
    ///     Indexes this instance.
    /// </summary>
    /// <returns></returns>
    public IActionResult Index()
    {
        try
        {
            var dashboardViewModel = _dashboardService.GetDashboardViewModel();

            _logger.Trace("Job List is rendered successfully.");
            return View(dashboardViewModel);
        }
        catch (Exception e)
        {
            _logger.Error(ErrorHandling.DefaultException(e.Message));
            return StatusCode(500, "Something went wrong.");
        }
    }


    /// <summary>
    ///     Assigns the users view.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    [HttpGet]
    public IActionResult AssignUsersView(int id)
    {
        try
        {
            var users = _userService.GetAllUsersWithLinkStatus(id);
            var jobOpeningTitle = _jobOpeningService.GetJobOpeningTitleById(id);
            var jobOpening = new JobOpeningBasicViewModel
            {
                Id = id,
                Title = jobOpeningTitle
            };

            var viewModel = new AssignUsersViewModel
            {
                JobOpening = jobOpening,
                Users = users
            };

            if (viewModel == null)
            {
                _logger.Error("Failed to create AssignUsersViewModel");
                return NotFound();
            }

            _logger.Trace("Successfully created AssignUsersViewModel for JobOpening [" + id + "].");


            return PartialView("~/Views/Dashboard/_AssignUsersView.cshtml", viewModel);
        }
        catch (Exception e)
        {
            _logger.Error(ErrorHandling.DefaultException(e.Message));
            return StatusCode(500, "Something went wrong.");
        }
    }

    /// <summary>
    ///     Updates the job opening assignments.
    /// </summary>
    /// <param name="assignedUserIds">The assigned user ids.</param>
    /// <param name="jobOpeningId">The job opening identifier.</param>
    /// <returns></returns>
    [HttpPost]
    public IActionResult AssignUserViewUpdate(List<string> assignedUserIds, int jobOpeningId)
    {
        try
        {
            _jobOpeningService.UpdateJobOpeningUsers(jobOpeningId, assignedUserIds);
            return Ok();
        }
        catch (Exception e)
        {
            _logger.Error(ErrorHandling.DefaultException(e.Message));
            return StatusCode(500, "Something went wrong.");
        }
    }

    /// <summary>
    ///     Views the details.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    [HttpGet]
    public IActionResult ViewDetails(int id)
    {
        try
        {
            // Fetch the application details based on the applicationId
            var applicant = _applicantService.GetApplicantByIdAll(id);
            var application = applicant.Application;
            var jobOpening = _jobOpeningService.GetByIdClean(application.JobOpeningId);
            application.JobOpening = jobOpening;
            if (application == null)
                // Handle the case when the application is not found
                return NotFound(); // Return a 404 Not Found response

            // Pass the application object to the view
            return PartialView("~/Views/Dashboard/_ViewDetails.cshtml", application);
        }
        catch (Exception e)
        {
            _logger.Error(ErrorHandling.DefaultException(e.Message));
            return StatusCode(500, "Something went wrong.");
        }
    }

    /// <summary>
    ///     Views the details update.
    /// </summary>
    /// <param name="appId">The application identifier.</param>
    /// <param name="status">The status.</param>
    /// <returns></returns>
    public IActionResult ViewDetailsUpdate(Guid appId, string status)
    {
        try
        {
            var application = _dashboardService.GetApplicationById(appId);
            var user = _userManager.GetUserAsync(User).Result;
            var foundUser = _userService.GetByEmail(user.Email);
            
            _dashboardService.UpdateStatus(application, foundUser, status, "Approval");
            return RedirectToAction("DirectoryView");
        }
        catch (Exception e)
        {
            _logger.Error(ErrorHandling.DefaultException(e.Message));
            return StatusCode(500, "Something went wrong.");
        }
    }

    /// <summary>
    ///     Downloads the file.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    public IActionResult DownloadFile(int id)
    {
        var applicant = _applicantService.GetApplicantByIdAll(id);

        if (applicant?.CV != null)
            // Assuming the file is stored as a byte array named "FileData"
            return File(applicant.CV, "application/octet-stream", "resume.pdf");

        return NotFound();
    }

    /// <summary>
    ///     Retrieves the data needed to display the Applicant Directory view, which includes a list of applicants along with
    ///     their job titles,
    ///     and information about shortlisted applicants for HR and Technical positions.
    /// </summary>
    /// <returns>An IActionResult representing the Applicant Directory view populated with the required data.</returns>
    public async Task<IActionResult> DirectoryView()
    {
        try
        {
            var userAspId = _userManager.GetUserId(User);
            var user = await _userManager.GetUserAsync(User);
            var applicantDirectoryViewModel = await _dashboardService.GetDirectoryViewModel(user.Email, userAspId);

            if (applicantDirectoryViewModel.JobOpenings == null)
            {
                applicantDirectoryViewModel = new ApplicantDirectoryViewModel();
                _toastNotification.AddWarningToastMessage("Directory View Model - JobOpenings not found.");
            }

            return View(applicantDirectoryViewModel);
        }
        catch (Exception e)
        {
            _logger.Error(ErrorHandling.DefaultException(e.Message));
            _toastNotification.AddErrorToastMessage(e.Message);
            return RedirectToAction("DirectoryView");
        }
    }

    public async Task<IActionResult> JobOpeningsView(int jobId)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            var directoryViewModel = _dashboardService.GetApplicantDirectoryViewModel(user.Email, jobId);
            if (directoryViewModel == null)
            {
                directoryViewModel = new ApplicantDirectoryViewModel();
                _toastNotification.AddWarningToastMessage("Directory View Model is not found.");
            }

            return View(directoryViewModel);
        }
        catch (Exception e)
        {
            _logger.Error(ErrorHandling.DefaultException(e.Message));
            _toastNotification.AddErrorToastMessage(e.Message);
            return RedirectToAction("DirectoryView");
        }
    }

    [HttpPost]
    public IActionResult UpdateScore(int examinationId, int percentage)
    {
        try
        {
            if (percentage < 0)
            {
                _logger.Warn("Score percentage is invalid.");
                return BadRequest();
            }

            var data = _examinationService.UpdateExaminationScore(examinationId, percentage);
            if (!data.Result)
            {
                _logger.Trace($"Successfully updated the score of Examination [ {examinationId} ].");
                return Ok();
            }
            else
            {
                _logger.Error(ErrorHandling.SetLog(data));
                return BadRequest();
            }
        }
        catch (Exception e)
        {
            _logger.Error(ErrorHandling.DefaultException(e.Message));
            return StatusCode(500, "Something went wrong.");
        }
    }

    /// <summary>
    ///     Action method for displaying the details of an applicant and their associated character references.
    /// </summary>
    /// <returns>Returns a View containing the applicant details and character references.</returns>
    public IActionResult DirectoryViewDetails(int applicantId)
    {
        try
        {
            var applicant = new Applicant();
            var characterReferences = new List<CharacterReference>();
            var background = new List<BackgroundCheck>();
            applicant = _applicantService.GetApplicantByIdAll(applicantId);
            characterReferences = _characterReferenceService.GetReferencesByApplicantId(applicantId);


            foreach (var characterReference in characterReferences)
            {
                background.Add(_backgroundCheckService.GetBackgroundByCharacterRefId(characterReference.Id));
            }

            var model = new ApplicantDetailsViewModel
            {
                Applicant = applicant,
                Status = applicant.Application?.Status,
                UpdateDate = applicant.Application?.UpdateTime,
                CharacterReferences = characterReferences,
                BackgroundCheck = background
            };

            return View(model);
        }
        catch (Exception e)
        {
            _logger.Error(ErrorHandling.DefaultException(e.Message));
            return StatusCode(500, "Something went wrong.");
        }
    }

    /// <summary>
    ///     Action method for downloading the resume of an applicant identified by the given 'applicantId'.
    /// </summary>
    /// <param name="applicantId">The unique identifier of the applicant whose resume is to be downloaded.</param>
    /// <returns>Returns a FileResult containing the applicant's resume in PDF format for download.</returns>
    public IActionResult DownloadResume(int applicantId)
    {
        try
        {
            var applicant = _applicantService.GetApplicantById(applicantId);
            var fileName = $"{applicant.Firstname}_{applicant.Lastname}_Resume.pdf";

            return File(applicant.CV, "application/pdf", fileName);
        }
        catch (Exception e)
        {
            _logger.Error(ErrorHandling.DefaultException(e.Message));
            return StatusCode(500, "Something went wrong.");
        }
    }

    /// <summary>
    ///     Requests a background check by sending an email to the specified character reference for a particular applicant.
    /// </summary>
    /// <param name="characterReferenceId">The unique identifier of the character reference to whom the request is sent.</param>
    /// <param name="applicantId">The unique identifier of the applicant for whom the background check is requested.</param>
    /// <returns>An IActionResult representing the result of the request.</returns>
    public IActionResult RequestBackgroundCheck(int characterReferenceId, int applicantId)
    {
        try
        {
            var userAspId = _userManager.GetUserId(User);
            var userId = _userService.GetUserIdByAspId(userAspId);
            var applicant = new Applicant();
            var characterReferences = new CharacterReference();
            applicant = _applicantService.GetApplicantById(applicantId);
            characterReferences = _characterReferenceService.GetCharacterReferenceById(characterReferenceId);
            _emailSendingService.SendRequestReference(characterReferences, applicant, userId);

            if (applicant == null || characterReferences == null) return NotFound();

            return RedirectToAction("DirectoryView");
        }
        catch (Exception e)
        {
            _logger.Error(ErrorHandling.DefaultException(e.Message));
            return StatusCode(500, "Something went wrong.");
        }
    }

    public async Task<IActionResult> CheckBackground(Guid appId)
    {
        try
        {
            var aspUser = await _userManager.GetUserAsync(User);
            var user = _userService.GetByEmail(aspUser.Email);
            var application = _applicationService.GetApplicationById(appId);
            _dashboardService.UpdateStatus(application, user, "Undergoing Background Check", "");
            _toastNotification.AddSuccessToastMessage("Successfully changed the status.");
            BackgroundJob.Schedule(
                () => _dashboardService.SendListEmail(application.Applicant.Id, aspUser.Email, user.Fullname),
                TimeSpan.FromHours(48));
            return RedirectToAction("DirectoryView");
        }
        catch (Exception e)
        {
            _logger.Error(ErrorHandling.DefaultException(e.Message));
            _toastNotification.AddErrorToastMessage(e.Message);
            return RedirectToAction("DirectoryView");
        }
    }

    public IActionResult UpdateApplicationStatus(Guid applicationId, string status)
    {
        try
        {
            var user = _userManager.GetUserAsync(User).Result;
            var foundUser = _userService.GetByEmail(user.Email);

            var application = _applicationService.GetApplicationById(applicationId);
            application = _trackService.UpdateApplicationStatus(application, foundUser, status, string.Empty);

            if (application != null)
            {
                var logContent = _applicationService.Update(application);
                if (!logContent.Result)
                    _logger.Trace($"Successfully updated application [ {application.Id} ].");
                else
                    _logger.Error(ErrorHandling.SetLog(logContent));
            }

            _toastNotification.AddSuccessToastMessage("Successfully changed the status.");
            return RedirectToAction("DirectoryView");
        }
        catch (Exception e)
        {
            _logger.Error(ErrorHandling.DefaultException(e.Message));
            _toastNotification.AddErrorToastMessage(e.Message);
            return RedirectToAction("DirectoryView");
        }  
    }
}