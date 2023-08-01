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
        private readonly TokenHelper _tokenHelper;

        public TrackerController(IApplicationService applicationService, IApplicantService applicantService, IUserService userService, ITrackService trackService, IConfiguration config)
        {
            _applicationService = applicationService;
            _applicantService = applicantService;
            _userService = userService;
            _trackService = trackService;
            _tokenHelper = new TokenHelper(config["TokenHelper:SecretKey"]);
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
        /// <param name="token">The token.</param>
        [Route("Tracker/ChangeStatus/{token}")]
        public IActionResult ChangeStatus(string token)
        {
            try
            {
                Dictionary<string, string> tokenClaims = _tokenHelper.GetTokenClaims(token, "ChangeStatus");
                if (tokenClaims.Count == 0)
                {
                    _logger.Warn("Invalid or expired token.");
                }

                int userId = int.Parse(tokenClaims["userId"]);
                Guid appId = Guid.Parse(tokenClaims["appId"]);
                string status = tokenClaims["newStatus"];
                string choice = tokenClaims["choice"];

                //Check if applicant and user exists
                var application = _applicationService.GetApplicationById(appId);
                var user = _userService.GetById(userId);

                if (application != null && user != null)
                {
                    var result = _trackService.UpdateApplicationStatusByEmailResponse(application, user, choice, status);
                    _applicationService.Update(result);
                }

                return RedirectToAction("ChangeStatusView");
            }
            catch (Exception e)
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