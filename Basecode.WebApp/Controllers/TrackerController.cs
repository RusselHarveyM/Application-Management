using Basecode.Services.Interfaces;
using Basecode.Services.Services;
using Microsoft.AspNetCore.Mvc;
using NLog;
using NToastNotify;

namespace Basecode.WebApp.Controllers;

public class TrackerController : Controller
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly IApplicationService _applicationService;
    private readonly IToastNotification _toastNotification;
    private readonly TokenHelper _tokenHelper;
    private readonly ITrackService _trackService;
    private readonly IUserService _userService;

    public TrackerController(IApplicationService applicationService, IUserService userService,
        ITrackService trackService, IConfiguration config, IToastNotification toastNotification)
    {
        _applicationService = applicationService;
        _userService = userService;
        _trackService = trackService;
        _tokenHelper = new TokenHelper(config["TokenHelper:SecretKey"]);
        _toastNotification = toastNotification;
    }

    /// <summary>
    ///     Indexes this instance.
    /// </summary>
    /// <returns>
    ///     A view of the tracker page
    /// </returns>
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    ///     Retrieves an application with the given Id
    /// </summary>
    /// <param name="id">The application ID.</param>
    /// <returns>
    ///     A view with either the tracker result table or an error message
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

            _logger.Trace("Application [" + id + "] found.");
            return View("Index", application);
        }
        catch (Exception e)
        {
            _logger.Error(ErrorHandling.DefaultException(e.Message));
            return StatusCode(500, "Something went wrong.");
        }
    }

    /// <summary>
    ///     Changes the status.
    /// </summary>
    /// <param name="token">The token.</param>
    [Route("Tracker/ChangeStatus/{token}")]
    public IActionResult ChangeStatus(string token)
    {
        try
        {
            var tokenClaims = _tokenHelper.GetTokenClaims(token, "ChangeStatus");
            if (tokenClaims.Count == 0) _logger.Warn("Invalid or expired token.");

            var userId = int.Parse(tokenClaims["userId"]);
            var appId = Guid.Parse(tokenClaims["appId"]);
            var status = tokenClaims["newStatus"];
            var choice = tokenClaims["choice"];

            //Check if applicant and user exists
            var application = _applicationService.GetApplicationById(appId);
            var user = _userService.GetById(userId);

            if (application != null && user != null)
            {
                var result = _trackService.UpdateApplicationStatusByEmailResponse(application, user, choice, status);
                if (status != "For Technical Interview")    // Need to wait for the shortlisting process after the Technical Interview
                {
                    _applicationService.Update(result);
                    _toastNotification.AddSuccessToastMessage("Status Successfully Changed.");
                }
            }

            return RedirectToAction("Index", "Home");
        }
        catch (Exception e)
        {
            _logger.Error(ErrorHandling.DefaultException(e.Message));
            _toastNotification.AddErrorToastMessage(e.Message);
            return RedirectToAction("Index", "Home");
        }
    }
}