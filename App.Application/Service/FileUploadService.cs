using App.Application.Interfaces.Services;
using App.Common.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using static App.Common.Helpers.HttpRequestHeaderManager;
namespace App.Application.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly HttpClient _httpClient;
        private readonly HttpRequestHeaderManager _headerManager;
        private readonly IConfiguration _configuration;


        public FileUploadService(HttpClient httpClient, HttpRequestHeaderManager headerManager, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _headerManager = headerManager;
            _configuration = configuration;
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            var apiBaseUrl = $"{_configuration["AppSettings:ApiBaseUrl"]}/UploadFile";

            using var formData = new MultipartFormDataContent();
            var fileContent = new StreamContent(file.OpenReadStream());
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            formData.Add(fileContent, "file", file.FileName);
            _headerManager.SetHeaders(_httpClient);
            var response = await _httpClient.PostAsync(apiBaseUrl, formData);
            if (!response.IsSuccessStatusCode)
            {
                var errorDetails = await response.Content.ReadAsStringAsync();
                throw new Exception($"File upload failed: {response.ReasonPhrase} - {errorDetails}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            dynamic responseObject = JsonConvert.DeserializeObject<dynamic>(responseContent);

            return responseObject?.file_url;
        }

    }
}
