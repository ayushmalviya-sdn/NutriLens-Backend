using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Common.Helpers
{
    public class HttpRequestHeaderManager
    {
        private readonly IConfiguration _configuration;

        public HttpRequestHeaderManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Method to add headers to HttpClient
        public void SetHeaders(HttpClient client)
        {
            client.DefaultRequestHeaders.Add("accept", "application/json");
            client.DefaultRequestHeaders.Add("authorization", Environment.GetEnvironmentVariable("API_AUTHTOKEN"));
            client.DefaultRequestHeaders.Add("origin", _configuration["AppSettings:OriginUrl"]);
            client.DefaultRequestHeaders.Add("referer", _configuration["AppSettings:RefererUrl"]);
            client.DefaultRequestHeaders.Add("x-app-id", _configuration["AppSettings:AppId"]);
            client.DefaultRequestHeaders.Add("x-origin-url", _configuration["AppSettings:RefererUrl"]);

        }
    }
}
