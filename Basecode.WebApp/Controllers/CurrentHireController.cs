using Basecode.Services.Interfaces;
using Basecode.Services.Services;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace Basecode.WebApp.Controllers;

public class CurrentHireController : Controller
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly ICurrentHireService _currentHireService;
    private readonly TokenHelper _tokenHelper;

    public CurrentHireController(ICurrentHireService currentHireService, IConfiguration config)
    {
        _currentHireService = currentHireService;
        _tokenHelper = new TokenHelper(config["TokenHelper:SecretKey"]);
    }

    /// <summary>
    ///     Accepts the job offer.
    /// </summary>
    [Route("CurrentHire/AcceptOffer/{token}")]
    public IActionResult AcceptOffer(string token)
    {
        try
        {
            ViewBag.IsHireAccepted = false;
            var tokenClaims = _tokenHelper.GetTokenClaims(token, "AcceptOffer");
            if (tokenClaims.Count == 0)
            {
                _logger.Warn("Invalid or expired token.");
                return View();
            }

            if (tokenClaims.TryGetValue("userId", out var userId))
            {
                var currentHireId = int.Parse(userId);
                var data = _currentHireService.AcceptOffer(currentHireId);
                if (!data.Result)
                {
                    _logger.Trace("User Offer [" + currentHireId + "] has been successfully accepted.");
                    ViewBag.IsHireAccepted = true;
                }
            }

            return View();
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
            ViewBag.IsHireRejected = false;
            var tokenClaims = _tokenHelper.GetTokenClaims(token, "RejectOffer");
            if (tokenClaims.Count == 0)
            {
                _logger.Warn("Invalid or expired token.");
                return View();
            }

            if (tokenClaims.TryGetValue("userId", out var userId))
            {
                var currentHireId = int.Parse(userId);
                var data = await _currentHireService.RejectOffer(currentHireId);
                if (!data.Result)
                {
                    _logger.Trace("User Offer [" + currentHireId + "] has been successfully rejected.");
                    ViewBag.IsHireRejected = true;
                }
            }

            return View();
        }
        catch (Exception e)
        {
            _logger.Error(ErrorHandling.DefaultException(e.Message));
            return StatusCode(500, "Something went wrong.");
        }
    }
}