using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace SWEN2_TourPlannerGroupProject.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        private static readonly ILoggerWrapper log = LoggerFactory.GetLogger();
        public AppDbContext CreateDbContext(string[] args)
        {
            log.Info("Creating AppDbContext for design-time operations.");
            // option to read connection string from appsettings.json to be able to update one db while running the other on app.
            // reads appsettings.json
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            var connectionStringName = "TestConnection"; // or "DefaultConnection" based on your setup
            var connectionString = config.GetConnectionString(connectionStringName);

            /* // this does not work, sad. we gotta change the connection string in the AppDbcontext class manually before migrating to the correct db.
            var connectionString = App.ConnectionString; // uses the same connection string as the app
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                // Fallback message if connection string is not set, that means the app was never started
                throw new InvalidOperationException(
                    "App.ConnectionString is not set. Please start the WPF application once to initialize the connection string before running EF migrations.");
            }
            */
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseNpgsql(connectionString);
            log.Info($"AppDbContext created with Database {connectionStringName} for design-time operations.");
            return new AppDbContext(optionsBuilder.Options);
        }
    }
}

