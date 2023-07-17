using Basecode.Main.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Basecode.Services.Interfaces;
using NLog;
using Basecode.Data.ViewModels;
using Basecode.Services.Services;
using Microsoft.IdentityModel.Tokens;

namespace Basecode.Main.Controllers
{
    public class HomeController : Controller
    {
        private readonly IJobOpeningService _jobOpeningService;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IApplicationService _applicationService;
        private readonly ITrackService _trackService;
        private readonly IApplicantService _applicantService;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController" /> class.
        /// </summary>
        /// <param name="jobOpeningService">The job opening service.</param>
        public HomeController(IJobOpeningService jobOpeningService, IApplicationService applicationService, ITrackService trackService,IApplicantService applicantService)
        {
            _jobOpeningService = jobOpeningService;
            _applicationService = applicationService;
            _trackService = trackService;
            _applicantService = applicantService;
        }

        /// <summary>
        /// Retrieves a list of job openings, category jobs and returns a view with the list.
        /// </summary>
        /// <returns>
        /// A view with a list of job openings and category of job.
        /// </returns>
        public async Task<IActionResult> Index()
        {
            try
            {
                // Get all jobs currently available.
                var jobOpenings = _jobOpeningService.GetJobs();
                var applicant = _applicantService.GetApplicantById(5003);

                await _trackService.UpdateTrackStatusEmail(
                            applicant,
                            new Guid("dc061db4-83bd-4d29-6b07-08db864fdfa7"),
                            5002,
                            "For Technical Interview",
                            "Approval");

                if (jobOpenings.IsNullOrEmpty())
                {
                    _logger.Error("No current jobs.");
                    return View(new List<JobOpeningViewModel>());
                }

                _logger.Trace("Get Jobs Successfully");
                return View(jobOpenings);
            }
            catch (Exception e)
            {
                _logger.Error(ErrorHandling.DefaultException(e.Message));
                return StatusCode(500, "Something went wrong.");
            }
        }
    }
}