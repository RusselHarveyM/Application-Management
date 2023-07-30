using Basecode.Services.Interfaces;
using Basecode.Services.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using NLog;
using RestSharp;

namespace Basecode.WebApp.Controllers;

public class OAuthController : Controller
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly IOAuthService _oAuthService;
    private readonly IConfiguration _config;

    private readonly IWebHostEnvironment _environment;
    private string tokensFile;

    public OAuthController(IOAuthService oAuthService,IConfiguration config, IWebHostEnvironment environment)
    {
        _oAuthService = oAuthService;
        _config = config;
        _environment = environment;
        tokensFile = _environment.ContentRootPath + @"\tokens.json";
    }

    public IActionResult Callback(string tenant, string state, string admin_consent)
    {

        try
        {
            var result = _oAuthService.Callback(tenant, state, admin_consent);
            if (result)
            {
                return RedirectToAction("Index", "Home");
            }

            return StatusCode(400);
        }
        catch (Exception e)
        {
            _logger.Error(ErrorHandling.DefaultException(e.Message));
            return StatusCode(500, "Something went wrong.");
        }
    }
    
    public ActionResult OauthRedirect()
    {
        var redirectUrl = "https://login.microsoftonline.com/common/adminconsent?" +
                          "&state=automationsystem2" +
                          "&redirect_uri=" + _config["GraphApi:Redirect_url"] +
                          "&client_id=" + _config["GraphApi:ClientId"];
        return Redirect(redirectUrl);
    }

}