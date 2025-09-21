using App.Application.Dto;
using App.Application.Interfaces.Services;
using App.Common.Helpers;
using Flurl.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
namespace App.Application.Service
{
    
    public class AIService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly HttpRequestHeaderManager _headerManager;

        public AIService(HttpClient httpClient, IConfiguration configuration, HttpRequestHeaderManager headerManager)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _headerManager = headerManager;
        }
      

        public async Task<dynamic> GenerateNutrientDetails(string fileUrl)
        {
           
            var apiBaseUrl = $"{_configuration["AppSettings:ApiBaseUrl"]}/InvokeLLM";
            var body = new
            {
                prompt = @"
        Analyze this food image and provide detailed nutritional information. 
        Identify the specific food item(s), estimate portion size, and calculate nutritional values.
        Be as accurate as possible with calorie estimation and macronutrient breakdown.
        Also suggest 3-5 healthier alternatives if the food is not particularly healthy.
        Rate the overall healthiness on a scale of 1-10 (1 being very unhealthy, 10 being very healthy).
        Provide a confidence score for your food identification (0-100%).
        ",
                file_urls = new[] { fileUrl },
                response_json_schema = new
                {
                    type = "object",
                    properties = new
                    {
                        food_name = new { type = "string" },
                        food_category = new { type = "string" },
                        calories_per_serving = new { type = "number" },
                        serving_size = new { type = "string" },
                        macros = new
                        {
                            type = "object",
                            properties = new
                            {
                                protein = new { type = "number" },
                                carbs = new { type = "number" },
                                fat = new { type = "number" },
                                fiber = new { type = "number" },
                                sugar = new { type = "number" }
                            }
                        },
                        healthiness_score = new { type = "number", minimum = 1, maximum = 10 },
                        healthier_alternatives = new
                        {
                            type = "array",
                            items = new { type = "string" }
                        },
                        confidence_score = new { type = "number", minimum = 0, maximum = 100 }
                    }
                }
            };

            try
            {
                //_headerManager.SetHeaders(_httpClient);
                var response = await apiBaseUrl
                    .WithHeader("accept", "application/json")
                    .WithHeader("accept-language", "en-GB,en-US;q=0.9,en;q=0.8")
                    .WithHeader("authorization", "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJjaWhveDM5MDQ3QGNhbWpvaW50LmNvbSIsImV4cCI6MTc1OTAzMjIwOSwiaWF0IjoxNzU4NDI3NDA5fQ.rqO27vA6CdlzUBF6xcoC-Qj0lpezQHA7YrTRhWQb2Jo")
                    .WithHeader("content-type", "application/json")
                    .WithHeader("origin", "https://preview--nutri-scan-c317378f.base44.app")
                    .WithHeader("priority", "u=1, i")
                    .WithHeader("referer", "https://preview--nutri-scan-c317378f.base44.app/")
                    .WithHeader("sec-ch-ua", "\"Chromium\";v=\"140\", \"Not=A?Brand\";v=\"24\", \"Google Chrome\";v=\"140\"")
                    .WithHeader("sec-ch-ua-mobile", "?1")
                    .WithHeader("sec-ch-ua-platform", "\"Android\"")
                    .WithHeader("sec-fetch-dest", "empty")
                    .WithHeader("sec-fetch-mode", "cors")
                    .WithHeader("sec-fetch-site", "same-site")
                    .WithHeader("user-agent", "Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/140.0.0.0 Mobile Safari/537.36")
                    .WithHeader("x-app-id", "68cee92b0ee6f13cc317378f")
                    .WithHeader("x-origin-url", "https://preview--nutri-scan-c317378f.base44.app/Scanner?hide_badge=true")
                    .PostJsonAsync(body);

                var result = await response.GetStringAsync();
                var aiResponse = JsonConvert.DeserializeObject<dynamic>(result);
                Console.WriteLine(result);
                return aiResponse;
            }
            catch (FlurlHttpException ex)
            {
                var error = await ex.GetResponseStringAsync();
                Console.WriteLine($"Error: {error}");
                return error;
            }
        }

        public async Task<JObject> GetAIResponseAsync(FoodData foodData)
        {
            string prompt = PrepareFoodPrompt(foodData);
            var requestBody = new
            {
                model = "meta-llama/llama-3-8b-instruct",
                messages = new[]
        {
            new { role = "system", content = "You are a nutrition assistant. Always return only valid JSON following the user’s schema." },
            new { role = "user", content = prompt }
        },
                temperature = 0.7,
                max_tokens = 200
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_configuration["LLM:_apiKey"]),
                Content = content
            };

            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _configuration["LLM:_apiKey"]);
            request.Headers.Add("HTTP-Referer", "localhost");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            var apiResponse = JsonConvert.DeserializeObject<OpenAIResponse>(responseString);

            var rawContent = apiResponse?.Choices?[0]?.Message?.Content?.Trim();

            if (string.IsNullOrEmpty(rawContent))
                return null;

            try
            {
                return JObject.Parse(rawContent);
            }
            catch (JsonReaderException)
            {
                return null;
            }
        }
        private string PrepareFoodPrompt(FoodData foodData)
        {
            return $"What are the updated calories and minerals in {foodData.FoodName} with the following information: " +
                   $"Serving Size: {foodData.ServingSize}, " +
                   $"Protein: {foodData.Macros.Protein}g, " +
                   $"Carbs: {foodData.Macros.Carbs}g, " +
                   $"Fat: {foodData.Macros.Fat}g, " +
                   $"Fiber: {foodData.Macros.Fiber}g, " +
                   $"Sugar: {foodData.Macros.Sugar}g. " +
                   "Respond ONLY with valid JSON in this exact format: " +
                   "{ \"calories\": number, \"minerals\": { \"calcium\": \"value with units\", \"iron\": \"value with units\", \"magnesium\": \"value with units\", \"phosphorus\": \"value with units\", \"potassium\": \"value with units\", \"sodium\": \"value with units\", \"zinc\": \"value with units\", \"copper\": \"value with units\", \"manganese\": \"value with units\" } } " +
                   "Do not include vitamins or any other fields. Do not include extra text. Do not explain anything.";
        }
    }

}

