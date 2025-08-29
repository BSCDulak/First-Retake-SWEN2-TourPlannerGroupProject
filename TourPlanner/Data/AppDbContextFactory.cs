using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace SWEN2_TourPlannerGroupProject.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            /* option to read connection string from appsettings.json to be able to update one db while running the other on app.
            // reads appsettings.json
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var connectionString = config.GetConnectionString("TestConnection");
            */

            var connectionString = App.ConnectionString; // uses the same connection string as the app
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                // Fallback message if connection string is not set, that means the app was never started
                throw new InvalidOperationException(
                    "App.ConnectionString is not set. Please start the WPF application once to initialize the connection string before running EF migrations.");
            }
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}

