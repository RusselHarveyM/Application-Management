using Basecode.Data.Dto;
using Basecode.Services.Interfaces;
using Basecode.Services.Services;
using Basecode.Services.Util;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace Basecode.WebApp.Controllers;

public class CalendarController : Controller
{
    private readonly GraphHelper _graphHelper;
    private readonly UserManager<IdentityUser> _userManager;
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();


    public CalendarController(UserManager<IdentityUser> userManager, GraphHelper graphHelper)
    {
        _graphHelper = graphHelper;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<ActionResult> CreateEvent(CalendarEvent calendarEvent)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            var userEmail = await _userManager.GetEmailAsync(user);
            var joinUrl = _graphHelper.TestSetCalendar(calendarEvent);
            if (!joinUrl.Equals(""))
            {
                _logger.Trace("Event Created Successfully!");
                return RedirectToAction("Index", "Home");
            }
            _logger.Warn("Event Created Unsuccessfully.");
            return RedirectToAction("Index", "Home");
        }
        catch (Exception e)
        {
            _logger.Error(ErrorHandling.DefaultException(e.Message));
            return StatusCode(500, "Something went wrong.");
        }
    }
}