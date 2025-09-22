using App.Application.Interfaces;
using App.Application.Interfaces.Repositories;
using App.Application.Interfaces.Services;
using App.Application.Service;
using App.Application.Services;
using App.Common.Helpers;
using App.Infrastructure.DBContext;
using App.Infrastructure.Repository;
using App.SharedConfigs.DBContext;
using System.Net.Http.Headers;

namespace App.Api.Configuration
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAppDependencies(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork<ApplicationDbContext>>();
            string llmApiKey = Environment.GetEnvironmentVariable("LLM_API_KEY");
            string AuthToken = Environment.GetEnvironmentVariable("API_AUTHTOKEN");
            if (string.IsNullOrEmpty(llmApiKey) || string.IsNullOrEmpty(AuthToken))
            {
                throw new InvalidOperationException("LLM_API_KEY or API_AUTHTOKEN environment variable is not set.");
            }

            services.AddSingleton<HttpRequestHeaderManager>();
            services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
            services.AddScoped<IFileUploadService, FileUploadService>();
            services.AddScoped<IAIService, AIService>();
            services.AddSingleton<Func<IServiceProvider, string>>(sp =>
            {
                var configDb = sp.GetRequiredService<MasterDbContext>();
                var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
                var httpContext = httpContextAccessor.HttpContext ?? throw new Exception("No active HTTP context found.");

                // Get industry name from header
                var industryName = httpContext.Request.Headers["Industry-Name"].FirstOrDefault();
                var targetConnectionString = configDb.IndustryConfig
                    .Where(c => c.Name == industryName)
                    .Select(c => c.ConnectionString)
                    .FirstOrDefault();

                if (string.IsNullOrEmpty(targetConnectionString))
                {
                    throw new Exception($"AppDb connection string not found for industry '{industryName}'.");
                }

                return targetConnectionString;
            });
            services.AddHttpClient();
            return services;
        }
    }
}