using Basecode.Data.ViewModels;
using Basecode.Services.Interfaces;
using Basecode.Services.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NLog;

namespace Basecode.Main.Controllers;

public class HomeController : Controller
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly IJobOpeningService _jobOpeningService;

    /// <summary>
    ///     Initializes a new instance of the <see cref="HomeController" /> class.
    /// </summary>
    /// <param name="jobOpeningService">The job opening service.</param>
    public HomeController(IJobOpeningService jobOpeningService)
    {
        _jobOpeningService = jobOpeningService;
    }


    /// <summary>
    ///     Retrieves a list of job openings, category jobs and returns a view with the list.
    /// </summary>
    /// <returns>
    ///     A view with a list of job openings and category of job.
    /// </returns>
    public async Task<IActionResult> Index()
    {
        try
        {
            // Get all jobs currently available.
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