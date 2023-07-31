using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Basecode.WebApp.Controllers;

public class OAuthController : Controller
{
    private readonly IConfiguration _config;
    private readonly IWebHostEnvironment _environment;
    private string tokensFile;

    public OAuthController(IConfiguration config, IWebHostEnvironment environment)
    {
        _config = config;
        _environment = environment;
        tokensFile = _environment.ContentRootPath + @"\tokens.json";
    }

    public IActionResult Callback(string tenant, string state, string admin_consent)
    {
        if (!string.IsNullOrWhiteSpace(tenant))
        {
            RestClient restClient = new RestClient($"https://login.microsoftonline.com/{tenant}/oauth2/v2.0");
            RestRequest restRequest = new RestRequest("/token", Method.Post);

            restRequest.AddParameter("client_id", _config["GraphApi:ClientId"].ToString());
            restRequest.AddParameter("scope", _config["GraphApi:Scopes"].ToString());
            restRequest.AddParameter("grant_type", "client_credentials");
            restRequest.AddParameter("client_secret", _config["GraphApi:ClientSecret"].ToString());

            var response = restClient.Post(restRequest);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                System.IO.File.WriteAllText(tokensFile, response.Content);
                return RedirectToAction("Index", "Home");
            }
        }

        return RedirectToAction("Error");
    }
}