using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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

        private async Task<string> ParseResume()
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

        public async Task CheckResume()
        {
            var parsedResume = await ParseResume();

            var openAiApiKey = configuration["ApiKeys:OpenAI"];
            if (string.IsNullOrEmpty(openAiApiKey))
            {
                throw new ApplicationException("OpenAI API key not found in configuration.");
            }

            var prompt = "You are tasked with acting as an AI-powered resume checker for a hiring company looking to fill a " +
                        "Frontend Developer position. The input to your system is a parsed resume, and your goal is to score the resume based on how well it fits the given job description. " +
                        "The output should be a key-value pair of (\"Score\": Result) representing the overall likelihood of the resume fitting the Frontend Developer role," +
                        " expressed as a percentage between 1% to 100%.:: " + parsedResume;

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", openAiApiKey);

            var requestData = new
            {
                prompt = prompt,
                max_tokens = 150,
                temperature = 0.5
            };

            var jsonContent = JsonConvert.SerializeObject(requestData);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            using (var response = await httpClient.PostAsync("https://api.openai.com/v1/engines/davinci/completions", httpContent))
            {
                var responseString = await response.Content.ReadAsStringAsync();
                dynamic responseObject = JsonConvert.DeserializeObject(responseString);
                string result = responseObject.choices[0].text;
                Console.WriteLine(result);
            }
        }


    }
}
