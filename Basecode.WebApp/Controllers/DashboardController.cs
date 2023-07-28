using Basecode.Data.Models;
using Basecode.Data.ViewModels;
using Basecode.Services.Interfaces;
using Basecode.Services.Services;
using Microsoft.AspNetCore.Authorization;
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
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IDashboardService _dashboardService;

        public DashboardController(IApplicantService applicantService, IJobOpeningService jobOpeningService, IUserService userService, IDashboardService  dashboardService )
        {
            _applicantService = applicantService;
            _jobOpeningService = jobOpeningService;
            _userService = userService;
            _dashboardService = dashboardService;
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
        public async Task<IActionResult> ViewDetailsUpdate(Guid appId, string email, string status)
        {
            try
            {
                var application = _dashboardService.GetApplicationById(appId);
                var foundUser = _userService.GetByEmail(email);
                 await _dashboardService.UpdateStatus(application, foundUser, status);
                 return RedirectToAction("ShortListView");
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
                var applicants = _applicantService.GetApplicantNameAndJobTitle();

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
    }
}
