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

        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardController"/> class.
        /// </summary>
        /// <param name="applicantService">The applicant service.</param>
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
                //var data = _userService.GetById(id);
                //var jobOpenings = _jobOpeningService.GetById(id);

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

                // Use switch case for different roles
                //switch (data.Role)
                //{
                //    case "Deployment Team":
                //        _emailService.ScheduleInterview(data.Email, data.Fullname, data.Username, data.Password, jobOpenings.Title);
                //        break;

                //    case "Human Resources":
                //        _emailService.ScheduleForHR(data.Email, data.Fullname, data.Username, data.Password, jobOpenings.Title);
                //        break;

                //    case "Technical":
                //        _emailService.ScheduleForTechnical(data.Email, data.Fullname, data.Username, data.Password, jobOpenings.Title);
                //        break;

                //    default:
                //        // Handle the default case if the role doesn't match any case
                //        break;
                //}

                return PartialView("~/Views/Dashboard/_AssignUsersView.cshtml", viewModel);
            }
            catch (Exception e)
            {
                _logger.Error(ErrorHandling.DefaultException(e.Message));
                return StatusCode(500, "Something went wrong.");
            }
        }

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

        public IActionResult ViewDetailsUpdate(Guid appId, string email, string status)
        {
            var application = _dashboardService.GetApplicationById(appId);
            var foundUser = _userService.GetByEmail(email);

            _dashboardService.UpdateStatus(application, foundUser, status);

            return RedirectToAction("ShortListView");
        }


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

    }
}
