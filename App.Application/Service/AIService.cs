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
                        Provide a confidence score for your food identification (0-100%).",
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
                    .WithHeader("authorization", $"Bearer {Environment.GetEnvironmentVariable("API_AUTHTOKEN")}")
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
                max_tokens = 300
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_configuration["LLM:_apiUrl"]),
                Content = content
            };

            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Environment.GetEnvironmentVariable("LLM_API_KEY"));
            request.Headers.Add("HTTP-Referer", "localhost");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            var apiResponse = JsonConvert.DeserializeObject<OpenAIResponse>(responseString);

            var rawContent = apiResponse?.Choices?[0]?.Message?.Content?.Trim();

            if (string.IsNullOrEmpty(rawContent))
                return null;

            if (rawContent.StartsWith("```json"))
                rawContent = rawContent.Substring(7).Trim();
            else if (rawContent.StartsWith("```"))
                rawContent = rawContent.Substring(3).Trim();

            if (rawContent.EndsWith("```"))
                rawContent = rawContent.Substring(0, rawContent.Length - 3).Trim();

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
            var macros = foodData.Macros;

            return
                $"You are a nutrition assistant. You are given the name of a food and its nutritional values **per 100 grams (if measurable)** or **per 1 item (if countable)**.\n\n" +

                $"Food Name: \"{foodData.FoodName}\"\n" +
                $"Serving Size: {foodData.ServingSize}\n\n" +
                $"Nutritional values:\n" +
                $"- Protein: {macros.Protein}g\n" +
                $"- Carbs: {macros.Carbs}g\n" +
                $"- Fat: {macros.Fat}g\n" +
                $"- Fiber: {macros.Fiber}g\n" +
                $"- Sugar: {macros.Sugar}g\n" +
                $"- Calories: {foodData.CaloriesPerServing} kcal\n\n" +

                "**Rules:**\n" +
                "1. If the food is **measurable** (e.g., grams, milliliters), assume the values are per 100 grams and scale them based on the numeric weight provided.\n" +
                "2. If the food is **countable** (e.g., 2 bananas, 3 eggs), assume the values are per 1 item and multiply all macros and calories by the item count.\n" +
                "3. Use this formula to calculate calories if needed:\n" +
                "   - Protein: 4 kcal/g\n" +
                "   - Carbs: 4 kcal/g\n" +
                "   - Fat: 9 kcal/g\n" +
                "4. Round all values to a maximum of 2 decimal places.\n" +
                "5. Return only valid JSON. Do not include any explanation or extra text.\n\n" +

                "Use this format:\n" +
                "```json\n" +
                "{\n" +
                "  \"corrected_calories\": number,\n" +
                "  \"corrected_serving_size\": number,\n" +
                "  \"corrected_macros\": {\n" +
                "    \"protein\": number,\n" +
                "    \"carbs\": number,\n" +
                "    \"fat\": number,\n" +
                "    \"fiber\": number,\n" +
                "    \"sugar\": number\n" +
                "  }\n" +
                "}\n" +
                "```";
        }
    }

}

