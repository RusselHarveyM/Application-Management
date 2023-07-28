using Basecode.Data.Dto;
using Basecode.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Basecode.Domain;

public class CalendarService : ICalendarService
{
    private readonly IConfiguration _config;
    private readonly IWebHostEnvironment _environment;
    private string tokensFile;

    public CalendarService(IConfiguration config, IWebHostEnvironment environment)
    {
        _config = config;
        _environment = environment;
        tokensFile = _environment.ContentRootPath + @"\tokens.json";
    }

    public string CreateEvent(CalendarEvent calendarEvent, string email)
    {
        var tokens = JObject.Parse(File.ReadAllText(tokensFile));
        //var user = _config["GraphApi:ObjectId"];
        
        var restClient = new RestClient($"https://graph.microsoft.com/v1.0/users/{email}/calendar/events");
        var restRequest = new RestRequest();

        restRequest.AddHeader("Authorization", "Bearer " + tokens["access_token"].ToString());
        restRequest.AddHeader("Content-Type", "application/json");
        restRequest.AddParameter("application/json", JsonConvert.SerializeObject(calendarEvent), ParameterType.RequestBody);

        var response = restClient.Post(restRequest);

        if (response.StatusCode == System.Net.HttpStatusCode.Created)
        {
            if (response.Content != null)
            {
                var content = JObject.Parse(response.Content);
                var joinUrl = content["onlineMeeting"]?["joinUrl"]?.ToString();
                return joinUrl;
            }
        }

        return "";
    }

}