using Basecode.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using RestSharp;

namespace Basecode.Services.Services;

public class OAuthService : IOAuthService
{
    private readonly IConfiguration _config;
    private readonly string tokensFile = @"C:\Users\Lenovo\Documents\vs_projects\newtemp\Application-Management\Basecode.WebApp\tokens.json";
    

    public OAuthService(IConfiguration config)
    {
        _config = config;
    }
    
    public bool Callback(string tenant, string state, string admin_consent)
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
                return true;
            }
        }

        return false;
    }
}