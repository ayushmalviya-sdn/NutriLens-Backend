using System.Net.Http.Headers;
using System.Text;
using App.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace App.Application.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public FileUploadService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            var url = "https://base44.app/api/apps/68b95bb9494794ee2785144b/integration-endpoints/Core/UploadFile";

            using var formData = new MultipartFormDataContent();
            var fileContent = new StreamContent(file.OpenReadStream());
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            formData.Add(fileContent, "file", file.FileName);

            // Add headers
            _httpClient.DefaultRequestHeaders.Add("accept", "application/json");
            _httpClient.DefaultRequestHeaders.Add("accept-language", "en-GB,en-US;q=0.9,en;q=0.8");
            _httpClient.DefaultRequestHeaders.Add("authorization", "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJnYXlpdG9nMjM2QGlzaGVuc2UuY29tIiwiZXhwIjoxNzU4NzAzMzYwLCJpYXQiOjE3NTgwOTg1NjB9.0V3KEXqSZONS9XK4loai5TH1lbQ5jMdfasu4ZqFaLeU");
            _httpClient.DefaultRequestHeaders.Add("origin", "https://nutri-lens-2785144b.base44.app");
            _httpClient.DefaultRequestHeaders.Add("referer", "https://nutri-lens-2785144b.base44.app/");
            _httpClient.DefaultRequestHeaders.Add("x-app-id", "68b95bb9494794ee2785144b");
            _httpClient.DefaultRequestHeaders.Add("x-origin-url", "https://nutri-lens-2785144b.base44.app/");

            var response = await _httpClient.PostAsync(url, formData);

            if (!response.IsSuccessStatusCode)
            {


                throw new Exception($"File upload failed: {response.ReasonPhrase}");
            }
            var responseContent = await response.Content.ReadAsStringAsync();
            dynamic responseObject = JsonConvert.DeserializeObject<dynamic>(responseContent);

            return responseObject?.file_url;
        }
    }
}
