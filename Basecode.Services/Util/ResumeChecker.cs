using System.Net.Http.Headers;
using System.Text;
using Basecode.Data.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Basecode.Services.Util;

public class ResumeChecker
{
    private static readonly HttpClient client = new();
    private readonly IConfiguration configuration;

    public ResumeChecker(IConfiguration configuration)
    {
        this.configuration = configuration;
    }


    /// <summary>
    ///     Parses the resume.
    /// </summary>
    /// <param name="resume">The resume.</param>
    /// <returns></returns>
    /// <exception cref="System.ApplicationException">API key not found in configuration.</exception>
    private async Task<string> ParseResume(byte[] resume)
    {
        var apiKey = configuration["ApiKeys:Affinda"];
        if (string.IsNullOrEmpty(apiKey)) throw new ApplicationException("API key not found in configuration.");

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        var result = "";
        var content = new MultipartFormDataContent();
        content.Add(new ByteArrayContent(resume), "file", "resume.pdf");

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri("https://api.affinda.com/v2/resumes"),
            Headers =
            {
                { "accept", "application/json" }
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

    /// <summary>
    ///     Checks the resume.
    /// </summary>
    /// <param name="jobPosition">The job position.</param>
    /// <param name="resume">The resume.</param>
    /// <returns></returns>
    /// <exception cref="System.ApplicationException">OpenAI API key not found in configuration.</exception>
    public async Task<string> CheckResume(string jobPosition, List<Qualification> qualifications, byte[] resume)
    {
        var parsedResume = await ParseResume(resume);

        if (!(qualifications.Count > 0)) return "";

        var contents =
            $"As an AI-powered resume evaluator for a leading hiring company, your task is to analyze parsed resumes and assess their suitability for a specific job position - the {jobPosition} role." +
            $" Your objective is to return a JSON object with key-value pairs representing the overall likelihood of the resume aligning with the requirements of the {jobPosition} position." +
            $" The output JSON should have the following structure: {{\r\n  \"JobPosition\": \"{jobPosition}\",\r\n  \"Score\": \"Result\",\r\n  \"Explanation\": \"explanation\"\r\n}}\r\n. The \"Score\" value should be presented as a percentage, ranging from 1% to 100%." +
            $" Your advanced algorithms will thoroughly evaluate the resume's contents, focusing on essential skills, qualifications, and experience relevant to the {jobPosition} position. Concentrate on the field of {jobPosition} position. " +
            $" Here are the qualifications needed to apply for this job: ";

        foreach (var qual in qualifications) contents += $"{{\r\n \"{qual.Description}\"\r\n}}";


        var openAiApiKey = configuration["ApiKeys:OpenAI"];
        if (string.IsNullOrEmpty(openAiApiKey))
            throw new ApplicationException("OpenAI API key not found in configuration.");

        var maxContextLength = 4097;
        var truncatedParsedResume = parsedResume.Substring(0, Math.Min(parsedResume.Length, maxContextLength));


        // Set up the HttpClient
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", openAiApiKey);

        var requestBody = new
        {
            model = "gpt-3.5-turbo",
            messages = new[]
            {
                new
                {
                    role = "system",
                    content = contents +
                              $". Please provide a brief explanation of why the given score was assigned to the resume in the \"Explanation\" field.\r\n\r\nYour assessment should take into account the candidate's expertise, achievements, and experience specifically in the {jobPosition} field, determining its compatibility with the desired role."
                },
                new
                {
                    role = "user",
                    content = truncatedParsedResume // Use the parsedResume as user input
                }
            },
            max_tokens = 150
        };

        var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

        // Send the request to the OpenAI API using the correct endpoint for chat models
        var response = await httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);


        // Parse the response
        var responseContent = await response.Content.ReadAsStringAsync();
        var json = JObject.Parse(responseContent);

        // Accessing the "Score" value
        var cont = json["choices"]?[0]?["message"]?["content"]?.ToString();

        return cont;
    }
}