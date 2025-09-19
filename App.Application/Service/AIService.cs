using App.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace App.Application.Service
{
    // C# classes to mirror the expected JSON structure with explicit property name mapping.

    /// <summary>
    /// Represents the properties of a JSON schema object.
    /// </summary>
    public class JsonSchemaProperty
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        // The items property can be either a primitive schema (like "string") or a complex object schema.
        // We use an object type and let the serializer handle the correct nesting.
        // The previous code had issues with this, so we ensure proper class structure.
        [JsonPropertyName("items")]
        public object Items { get; set; }
    }

    /// <summary>
    /// Represents a nested schema for an item within the "healthier_alternatives" array.
    /// </summary>
    public class HealthierAlternativeSchema
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "object";

        [JsonPropertyName("properties")]
        public Dictionary<string, JsonSchemaProperty> Properties { get; set; }
    }

    /// <summary>
    /// Represents the main JSON schema for the request.
    /// </summary>
    public class JsonSchema
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "object";

        [JsonPropertyName("properties")]
        public Dictionary<string, JsonSchemaProperty> Properties { get; set; }
    }

    /// <summary>
    /// Represents the complete request body sent to the AI service.
    /// </summary>
    public class RequestBody
    {
        [JsonPropertyName("file_url")]
        public string FileUrl { get; set; }

        [JsonPropertyName("json_schema")]
        public JsonSchema JsonSchema { get; set; }
    }

    public class AIService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public AIService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<dynamic> GenerateNutrientDetails(string fileUrl)
        {
            var aiUrl = "https://base44.app/api/apps/68b95bb9494794ee2785144b/integration-endpoints/Core/ExtractDataFromUploadedFile";

            // Construct the JSON body for the request using the refined classes
            var requestBody = "\"{\\\"file_url\\\":\\\"https://base44.app/api/apps/68b95bb9494794ee2785144b/files/public/68b95bb9494794ee2785144b/9fd6e6b97_download.png\\\",\\\"json_schema\\\":{\\\"type\\\":\\\"object\\\",\\\"properties\\\":{\\\"food_name\\\":{\\\"type\\\":\\\"string\\\"},\\\"serving_size\\\":{\\\"type\\\":\\\"string\\\"},\\\"calories_per_serving\\\":{\\\"type\\\":\\\"number\\\"},\\\"protein\\\":{\\\"type\\\":\\\"number\\\"},\\\"carbs\\\":{\\\"type\\\":\\\"number\\\"},\\\"fat\\\":{\\\"type\\\":\\\"number\\\"},\\\"fiber\\\":{\\\"type\\\":\\\"number\\\"},\\\"sugar\\\":{\\\"type\\\":\\\"number\\\"},\\\"sodium\\\":{\\\"type\\\":\\\"number\\\"},\\\"ingredients\\\":{\\\"type\\\":\\\"array\\\",\\\"items\\\":{\\\"type\\\":\\\"string\\\"}},\\\"healthier_alternatives\\\":{\\\"type\\\":\\\"array\\\",\\\"items\\\":{\\\"type\\\":\\\"object\\\",\\\"properties\\\":{\\\"name\\\":{\\\"type\\\":\\\"string\\\"},\\\"reason\\\":{\\\"type\\\":\\\"string\\\"},\\\"calories_saved\\\":{\\\"type\\\":\\\"number\\\"}}}},\\\"health_score\\\":{\\\"type\\\":\\\"number\\\"}}}}\"";

            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, aiUrl)
            {
                Content = content
            };

            // Add headers to match the `curl` request exactly
            requestMessage.Headers.Add("accept", "application/json");
            requestMessage.Headers.Add("accept-language", "en-GB,en-US;q=0.9,en;q=0.8");
            requestMessage.Headers.Add("authorization", "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJoZXNhbGUxMTA5QGxhbmlwZS5jb20iLCJleHAiOjE3NTg2OTcyNjIsImlhdCI6MTc1ODA5MjQ2Mn0.5Lqm7FdxVA2GlcFs4tjE4EVpd24pTXCa85T8moWmc8A");
            requestMessage.Headers.Add("origin", "https://nutri-lens-2785144b.base44.app");
            requestMessage.Headers.Add("referer", "https://nutri-lens-2785144b.base44.app/");
            requestMessage.Headers.Add("sec-ch-ua", "\"Chromium\";v=\"140\", \"Not=A?Brand\";v=\"24\", \"Google Chrome\";v=\"140\"");
            requestMessage.Headers.Add("sec-ch-ua-mobile", "?0");
            requestMessage.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
            requestMessage.Headers.Add("sec-fetch-dest", "empty");
            requestMessage.Headers.Add("sec-fetch-mode", "cors");
            requestMessage.Headers.Add("sec-fetch-site", "same-site");
            requestMessage.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/140.0.0.0 Safari/537.36");
            requestMessage.Headers.Add("x-app-id", "68b95bb9494794ee2785144b");
            requestMessage.Headers.Add("x-origin-url", "https://nutri-lens-2785144b.base44.app/");

            var response = await _httpClient.SendAsync(requestMessage);

            if (!response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {response.StatusCode}, Response: {responseContent}");
                throw new Exception($"AI service call failed: {response.ReasonPhrase}. Response: {responseContent}");
            }

            var aiResponseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Response Content: " + aiResponseContent);

            var aiResponse = JsonSerializer.Deserialize<dynamic>(aiResponseContent);

            return aiResponse;
        }
    }
}
