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
        private readonly ITrackService _trackService;

        public TrackerController(IApplicationService applicationService, IApplicantService applicantService, IUserService userService, ITrackService trackService)
        {
            _applicationService = applicationService;
            _applicantService = applicantService;
            _userService = userService;
            _trackService = trackService;
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns>
        /// A view of the tracker page
        /// </returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Retrieves an application with the given Id
        /// </summary>
        /// <param name="id">The application ID.</param>
        /// <returns>
        /// A view with either the tracker result table or an error message
        /// </returns>
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

        /// <summary>
        /// Changes the status.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="appId">The application identifier.</param>
        /// <param name="status">The status.</param>
        /// <param name="choice">The choice.</param>
        /// <returns></returns>
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
                    _trackService.UpdateApplicationStatusByEmailResponse(application, user, choice, status);
                }

                return RedirectToAction("ChangeStatusView");
            }
            catch(Exception e)
            {
                _logger.Error(ErrorHandling.DefaultException(e.Message));
                return StatusCode(500, "Something went wrong.");
            }
        }


        /// <summary>
        /// Changes the status view.
        /// </summary>
        /// <returns></returns>
        public IActionResult ChangeStatusView()
        {
            return View();
        }
    }
}