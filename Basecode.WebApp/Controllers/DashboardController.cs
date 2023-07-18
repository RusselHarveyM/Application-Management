using Basecode.Data.Models;
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
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardController"/> class.
        /// </summary>
        /// <param name="applicantService">The applicant service.</param>
        public DashboardController(IApplicantService applicantService)
        {
            _applicantService = applicantService;
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            try
            {
                List<Applicant> applicants = _applicantService.GetApplicants();
                if(applicants.IsNullOrEmpty())
                {
                    _logger.Trace("Applicant List is null or empty.");
                    return View(applicants);
                }
                _logger.Trace("Applicants List is rendered successfully.");
                return View(applicants);
            }
            catch (Exception e)
            {
                _logger.Error(ErrorHandling.DefaultException(e.Message));
                return StatusCode(500, "Something went wrong.");
            }
        }
    }
}
