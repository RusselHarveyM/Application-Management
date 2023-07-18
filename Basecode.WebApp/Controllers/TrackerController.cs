using Basecode.Services.Interfaces;
using Basecode.Services.Services;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace Basecode.WebApp.Controllers
{
    public class TrackerController : Controller
    {
        private readonly IApplicationService _applicationService;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IApplicantService _applicantService;
        private readonly IUserService _userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackerController"/> class.
        /// </summary>
        /// <param name="applicationService">The application service.</param>
        public TrackerController(IApplicationService applicationService, IApplicantService applicantService, IUserService userService)
        {
            _applicationService = applicationService;
            _applicantService = applicantService;
            _userService = userService;
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns>A view of the tracker page</returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Retrieves an application with the given Id
        /// </summary>
        /// <param name="id">The application ID.</param>
        /// <returns>A view with either the tracker result table or an error message</returns>
        [HttpGet]
        public IActionResult ResultView(Guid id)
        {
            try
            {
                var application = _applicationService.GetById(id);
                if (application == null)
                {
                    ViewData["ErrorMessage"] = "Application not found.";
                    _logger.Error("Application [" + id + "] not found!");
                    return View("Index");
                }
                else
                {
                    _logger.Trace("Application [" + id + "] found.");
                    return View("Index", application);
                }
            }
            catch (Exception e)
            {
                _logger.Error(ErrorHandling.DefaultException(e.Message));
                return StatusCode(500, "Something went wrong.");
            }
        }

        [Route("Tracker/ChangeStatus/{userId}/{appId}/{status}/{choice}")]
        public IActionResult ChangeStatus(int userId,Guid appId, string status, string choice)
        {
            try
            {
                //Check if applicant and user exists
                var application = _applicationService.GetApplicationById(appId);
                var user = _userService.GetById(userId);

                if(application != null && user != null)
                {
                    _applicantService.UpdateApplication(application, user, choice, status);
                }

                return RedirectToAction("ChangeStatusView");
            }
            catch(Exception e)
            {
                _logger.Error(ErrorHandling.DefaultException(e.Message));
                return StatusCode(500, "Something went wrong.");
            }
        }


        public IActionResult ChangeStatusView()
        {
            return View();
        }
    }
}