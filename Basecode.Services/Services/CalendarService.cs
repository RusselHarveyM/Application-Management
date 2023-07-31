using Basecode.Data.Dto;
using Basecode.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;



namespace Basecode.Domain;

public class CalendarService : ICalendarService
{
    private readonly IConfiguration _config;

    public CalendarService(IConfiguration config)
    {
        _config = config;
    }

    public async Task<string> CreateEvent(CalendarEvent calendarEvent, string email)
    {
        var tenantId = _config["GraphApi:TenantId"];
        string tokenEndpoint = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token";
        // create an httpclient
        using (HttpClient client = new HttpClient())
        {
            var requestContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id", _config["GraphApi:ClientId"]),
                new KeyValuePair<string, string>("client_secret", _config["GraphApi:ClientSecret"]),
                new KeyValuePair<string, string>("scope", "https://graph.microsoft.com/.default"),
            });
            // retrieve access token
            HttpResponseMessage response = await client.PostAsync(tokenEndpoint, requestContent);
            string responseContent = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
            string accessToken = tokenResponse.GetProperty("access_token").GetString();

            string endpoint = $"https://graph.microsoft.com/v1.0/users/{email}/calendar/events";

            var jsonMessage = JsonSerializer.Serialize(calendarEvent);
            var content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Content = content;

            HttpResponseMessage responseHttp = await client.SendAsync(request);
            string responseString = await responseHttp.Content.ReadAsStringAsync();

            if (responseHttp.IsSuccessStatusCode)
            {
                Console.WriteLine("Successful.");
                var cont = JObject.Parse(responseString);
                var joinUrl = cont["onlineMeeting"]?["joinUrl"]?.ToString();
                return joinUrl;
            }

            Console.WriteLine("Failed.");
            return "";
        }
    }

}