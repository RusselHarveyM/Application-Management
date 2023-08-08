using Basecode.Data.ViewModels;
using Basecode.Services.Services;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace Basecode.WebApp.Controllers;

public class LoginController : Controller
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    ///     Displays the login view
    /// </summary>
    /// <returns>The login view</returns>
    public IActionResult Index()
    {
        _logger.Trace("Redirected to Login Page");
        return View();
    }

    /// <summary>
    ///     Redirect to dashboard when sign-up successfully
    /// </summary>
    /// <returns>Redirected page to dashboard</returns>
    [HttpPost]
    public IActionResult Login(LoginViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors)) _logger.Error(error.ErrorMessage);

                return RedirectToAction("Index");
            }
            //to be implemented for next discussion Authorization & Authentication
        }
        catch (Exception e)
        {
            _logger.Error(ErrorHandling.DefaultException(e.Message));
            return StatusCode(500, "Something went wrong." + e.Message);
        }

        _logger.Trace("Login Successfully");
        return RedirectToAction("Index", "Dashboard");
    }
}