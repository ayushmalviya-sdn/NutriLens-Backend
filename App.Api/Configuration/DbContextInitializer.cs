using App.Common.Constant;
using App.Infrastructure.DBContext;
using App.SharedConfigs.DBContext;
using Microsoft.EntityFrameworkCore;

namespace App.Api.Configuration
{
    public static class DbContextInitializer
    {
        public static void AddConfiguredDbContexts(this IServiceCollection services, IConfiguration configuration)
        {
            var databaseProvider = configuration["DatabaseProvider"];
            var masterConnectionString = configuration.GetConnectionString("MasterConnection");

            if (databaseProvider == Constants.ProviderNameSqlServer)
            {
                services.AddDbContext<MasterDbContext>(options =>
                    options.UseSqlServer(masterConnectionString));

                services.AddDbContext<ApplicationDbContext>((sp, options) =>
                {
                    var connectionStringFactory = sp.GetRequiredService<Func<IServiceProvider, string>>();
                    options.UseSqlServer(connectionStringFactory(sp));
                });
            }
            else if (databaseProvider == Constants.ProviderNamePostgreSQL)
            {
                services.AddDbContext<MasterDbContext>(options =>
                    options.UseNpgsql(masterConnectionString));

                services.AddDbContext<ApplicationDbContext>((sp, options) =>
                {
                    var connectionStringFactory = sp.GetRequiredService<Func<IServiceProvider, string>>();
                    options.UseNpgsql(connectionStringFactory(sp));
                });
            }
        }
    }
}
