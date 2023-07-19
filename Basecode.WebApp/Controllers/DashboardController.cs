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

        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardController"/> class.
        /// </summary>
        /// <param name="applicantService">The applicant service.</param>
        public DashboardController(IApplicantService applicantService, IJobOpeningService jobOpeningService, IUserService userService)
        {
            _applicantService = applicantService;
            _jobOpeningService = jobOpeningService;
            _userService = userService;
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
                List<ApplicantStatusViewModel> applicants = _applicantService.GetApplicantsByJobOpeningId(id);
                List<HRUserViewModel> users = _userService.GetAllUsersWithLinkStatus(id);

                AssignUsersViewModel viewModel = new AssignUsersViewModel()
                {
                    JobOpeningId = id,
                    JobOpeningTitle = jobOpeningTitle,
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
    }
}
