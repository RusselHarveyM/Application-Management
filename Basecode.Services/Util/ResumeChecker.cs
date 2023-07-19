using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Basecode.Services.Util
{
    public class ResumeChecker
    {
        private static readonly HttpClient client = new HttpClient();
        private readonly IConfiguration configuration;

        public ResumeChecker(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<string> ParseResume()
        {

            var apiKey = configuration["ApiKeys:Affinda"];
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new ApplicationException("API key not found in configuration.");
            }

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            var result = "";
            var templatePath = Path.Combine("wwwroot", "template", "resume.pdf");
            var content = new MultipartFormDataContent();
            content.Add(new ByteArrayContent(File.ReadAllBytes(templatePath)), "file", "resume.pdf");

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://api.affinda.com/v2/resumes"),
                Headers =
                {
                    { "accept", "application/json" },
                 },
                Content = content
            };

            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                result = body;
            }

            return result;
        }
    }
}
