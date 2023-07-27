using Basecode.Data.Dto;
using Basecode.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Basecode.Domain;

public class CalendarService : ICalendarService
{
    private readonly IConfiguration _config;
    private string tokensFile = @"C:\Users\Lenovo\Documents\vs_projects\newtemp\Application-Management\Basecode.WebApp\tokens.json";
    

    public CalendarService(IConfiguration config)
    {
        _config = config;
    }

    public string CreateEvent(CalendarEvent calendarEvent)
    {
        var tokens = JObject.Parse(File.ReadAllText(tokensFile));
        var user = _config["GraphApi:ObjectId"];
        
        var restClient = new RestClient($"https://graph.microsoft.com/v1.0/users/{user}/calendar/events");
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