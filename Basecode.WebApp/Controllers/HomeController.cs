using Basecode.Main.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Basecode.Services.Interfaces;
using NLog;
using Basecode.Data.ViewModels;
using Basecode.Services.Services;
using Microsoft.IdentityModel.Tokens;
using Basecode.Services.Util;

namespace Basecode.Main.Controllers
{
    public class HomeController : Controller
    {
        private readonly IJobOpeningService _jobOpeningService;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController" /> class.
        /// </summary>
        /// <param name="jobOpeningService">The job opening service.</param>
        public HomeController(IJobOpeningService jobOpeningService)
        {
            _jobOpeningService = jobOpeningService;
        }

        /// <summary>
        /// Retrieves a list of job openings, category jobs and returns a view with the list.
        /// </summary>
        /// <returns>
        /// A view with a list of job openings and category of job.
        /// </returns>
        public IActionResult Index()
        {
            try
            {
                // Get all jobs currently available.
                var resumeChecker = new ResumeChecker();
                resumeChecker.ParseResume();
                var jobOpenings = _jobOpeningService.GetJobs();

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