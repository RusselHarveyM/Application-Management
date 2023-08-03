using Basecode.Data.Models;
using Basecode.Services.Interfaces;
using Basecode.Services.Services;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using NLog;
using NToastNotify;

namespace Basecode.WebApp.Controllers;

public class CurrentHireController : Controller
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly ICurrentHireService _currentHireService;
    private readonly TokenHelper _tokenHelper;
    private readonly ITrackService _trackService;
    private readonly IApplicationService _applicationService;
    private readonly IUserService _userService;
    private readonly IToastNotification _toastNotification;
    private readonly IScheduleSendingService _scheduleSendingService;
    private readonly IUserScheduleService _userScheduleService;
    private readonly IApplicantService _applicantService;

    public CurrentHireController(ICurrentHireService currentHireService, IConfiguration config,
        ITrackService trackService, IApplicationService applicationService, IUserService userService,
        IToastNotification toastNotification, IScheduleSendingService scheduleSendingService,
        IUserScheduleService userScheduleService, IApplicantService applicantService)
    {
        _currentHireService = currentHireService;
        _tokenHelper = new TokenHelper(config["TokenHelper:SecretKey"]);
        _trackService = trackService;
        _applicationService = applicationService;
        _userService = userService;
        _toastNotification = toastNotification;
        _scheduleSendingService = scheduleSendingService;
        _userScheduleService = userScheduleService;
        _applicantService = applicantService;
    }

    /// <summary>
    ///     Accepts the job offer.
    /// </summary>
    [Route("CurrentHire/AcceptOffer/{token}")]
    public IActionResult AcceptOffer(string token)
    {
        try
        {
            var tokenClaims = _tokenHelper.GetTokenClaims(token, "AcceptOffer");
            var result = _currentHireService.CurrentHireAcceptOffer(tokenClaims);
            if (result == -1)
            {
                _toastNotification.AddWarningToastMessage("Token Expired");
            }
            else
            {
                _toastNotification.AddSuccessToastMessage("Status Successfully Changed.");
            }

            return RedirectToAction("Index", "Home");
        }
        catch (Exception e)
        {
            _logger.Error(ErrorHandling.DefaultException(e.Message));
            return StatusCode(500, "Something went wrong.");
        }
    }

    /// <summary>
    ///     Rejects the job offer.
    /// </summary>
    [Route("CurrentHire/RejectOffer/{token}")]
    public async Task<IActionResult> RejectOffer(string token)
    {
        try
        {
            var tokenClaims = _tokenHelper.GetTokenClaims(token, "AcceptOffer");
            if (tokenClaims.Count == 0)
            {
                _logger.Warn("Invalid or expired token.");
                return RedirectToAction("Index", "Home");
            }

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
                _applicationService.Update(result);
            }

            _toastNotification.AddSuccessToastMessage("Status Successfully Changed.");
            return RedirectToAction("Index", "Home");
        }
        catch (Exception e)
        {
            _logger.Error(ErrorHandling.DefaultException(e.Message));
            return StatusCode(500, "Something went wrong.");
        }
    }
}