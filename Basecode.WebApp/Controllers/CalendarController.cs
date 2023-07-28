using Basecode.Data.Dto;
using Basecode.Services.Interfaces;
using Basecode.Services.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NLog;

namespace Basecode.WebApp.Controllers;

public class CalendarController : Controller
{
    private readonly ICalendarService _calendarService;
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();


    public CalendarController(ICalendarService calendarService)
    {
        _calendarService = calendarService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public ActionResult CreateEvent(CalendarEvent calendarEvent)
    {
        try
        {
            var joinUrl = _calendarService.CreateEvent(calendarEvent, "");
            if (!joinUrl.IsNullOrEmpty())
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