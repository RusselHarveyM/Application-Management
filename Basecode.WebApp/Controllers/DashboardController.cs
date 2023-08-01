using Basecode.Data.Models;
using Basecode.Data.ViewModels;
using Basecode.Data.ViewModels.Common;
using Basecode.Services.Interfaces;
using Basecode.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NLog;

namespace Basecode.WebApp.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IApplicantService _applicantService;
        private readonly IJobOpeningService _jobOpeningService;
        private readonly IUserService _userService;
        private readonly ICharacterReferenceService _characterReferenceService;
        private readonly IBackgroundCheckService _backgroundCheckService;
        private readonly IEmailSendingService _emailSendingService;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IDashboardService _dashboardService;
        private readonly UserManager<IdentityUser> _userManager;

        public DashboardController(IApplicantService applicantService, IJobOpeningService jobOpeningService, IUserService userService, ICharacterReferenceService characterReferenceService, IBackgroundCheckService backgroundCheckService, IEmailSendingService emailSendingService, IDashboardService  dashboardService, UserManager<IdentityUser> userManager)
        {
            _applicantService = applicantService;
            _jobOpeningService = jobOpeningService;
            _userService = userService;
            _characterReferenceService = characterReferenceService;
            _backgroundCheckService = backgroundCheckService;
            _emailSendingService = emailSendingService;
            _dashboardService = dashboardService;
            _userManager = userManager;
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            try
            {
                List<JobOpeningViewModel> jobs = _jobOpeningService.GetJobsWithApplications();
                if(jobs.IsNullOrEmpty())
                {
                    _logger.Info("Job List is null or empty.");
                    return View(new List<JobOpeningViewModel>());
                }
                _logger.Trace("Job List is rendered successfully.");
                return View(jobs);
            }
            catch (Exception e)
            {
                _logger.Error(ErrorHandling.DefaultException(e.Message));
                return StatusCode(500, "Something went wrong.");
            }
        }

        /// <summary>
        /// Assigns the users view.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult AssignUsersView(int id)
        {
            try
            {
                var jobOpeningTitle = _jobOpeningService.GetJobOpeningTitleById(id);
                JobOpeningBasicViewModel jobOpening = new JobOpeningBasicViewModel()
                {
                    Id = id,
                    Title = jobOpeningTitle,
                };
                List<ApplicantStatusViewModel> applicants = _applicantService.GetApplicantsByJobOpeningId(id);
                List<HRUserViewModel> users = _userService.GetAllUsersWithLinkStatus(id);

                AssignUsersViewModel viewModel = new AssignUsersViewModel()
                {
                    JobOpening = jobOpening,
                    Applicants = applicants,
                    Users = users,
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
        /// Updates the job opening assignments.
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
        /// Shorts the ListView.
        /// </summary>
        /// <returns></returns>
        public IActionResult ShortListView()
        {
            try
            {
                var shortlistedModel = new ShortListedViewModel();
                shortlistedModel.HRShortlisted = _dashboardService.GetShorlistedApplicatons("HR Shortlisted");
                shortlistedModel.TechShortlisted = _dashboardService.GetShorlistedApplicatons("Technical Shortlisted");

                return View(shortlistedModel);
            }
            catch (Exception e)
            {
                _logger.Error(ErrorHandling.DefaultException(e.Message));
                return StatusCode(500, "Something went wrong.");
            }
        }

        /// <summary>
        /// Views the details.
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
                {
                    // Handle the case when the application is not found
                    return NotFound(); // Return a 404 Not Found response
                }

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
        /// Views the details update.
        /// </summary>
        /// <param name="appId">The application identifier.</param>
        /// <param name="email">The email.</param>
        /// <param name="status">The status.</param>
        /// <returns></returns>
        public IActionResult ViewDetailsUpdate(Guid appId, string email, string status)
        {
            try
            {
                var application = _dashboardService.GetApplicationById(appId);
                var foundUser = _userService.GetByEmail(email);
                 _dashboardService.UpdateStatus(application, foundUser, status);
                 return RedirectToAction("ApplicantDirectoryView");
            }
            catch(Exception e)
            {
                _logger.Error(ErrorHandling.DefaultException(e.Message));
                return StatusCode(500, "Something went wrong.");
            }
        }

        /// <summary>
        /// Downloads the file.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public IActionResult DownloadFile(int id)
        {
            var applicant = _applicantService.GetApplicantByIdAll(id);

            if (applicant?.CV != null)
            {
                // Assuming the file is stored as a byte array named "FileData"
                return File(applicant.CV, "application/octet-stream", "resume.pdf");
            }

            return NotFound();
        }

        /// <summary>
        /// Retrieves the data needed to display the Applicant Directory view, which includes a list of applicants along with their job titles,
        /// and information about shortlisted applicants for HR and Technical positions.
        /// </summary>
        /// <returns>An IActionResult representing the Applicant Directory view populated with the required data.</returns>
        public IActionResult ApplicantDirectoryView()
        {
            try
            {
                var userAspId = _userManager.GetUserId(User);
                var applicants = _applicantService.GetApplicantsWithJobAndReferences(userAspId);

                var shortlistedModel = new ShortListedViewModel();
                shortlistedModel.HRShortlisted = _dashboardService.GetShorlistedApplicatons("HR Shortlisted");
                shortlistedModel.TechShortlisted = _dashboardService.GetShorlistedApplicatons("Technical Shortlisted");

                var applicantDirectoryViewModel = new ApplicantDirectoryViewModel
                {
                    Applicants = applicants,
                    Shortlists = shortlistedModel
                };

                return View(applicantDirectoryViewModel);
            }
            catch (Exception e)
            {
                _logger.Error(ErrorHandling.DefaultException(e.Message));
                return StatusCode(500, "Something went wrong.");
            }
        }

        /// <summary>
        /// Action method for displaying the details of an applicant and their associated character references.
        /// </summary>
        /// <returns>Returns a View containing the applicant details and character references.</returns>
        public IActionResult ApplicantViewDetails(int applicantId)
        {
            try
            {
                var applicant = new Applicant();
                var characterReferences = new List<CharacterReference>();
                var background = new List<BackgroundCheck>();
                applicant = _applicantService.GetApplicantById(applicantId);
                characterReferences = _characterReferenceService.GetReferencesByApplicantId(applicantId);

                foreach (var characterReference in characterReferences)
                {
                    background.Add(_backgroundCheckService.GetBackgroundByCharacterRefId(characterReference.Id));
                }

                var model = new ApplicantDetailsViewModel
                {
                    Applicant = applicant,
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
        /// Action method for downloading the resume of an applicant identified by the given 'applicantId'.
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
        /// Requests a background check by sending an email to the specified character reference for a particular applicant.
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

                if(applicant == null || characterReferences == null)
                {
                    return NotFound();
                }

                return RedirectToAction("ApplicantDirectoryView");
            }
            catch (Exception e)
            {
                _logger.Error(ErrorHandling.DefaultException(e.Message));
                return StatusCode(500, "Something went wrong.");
            }
        }
    }
}
