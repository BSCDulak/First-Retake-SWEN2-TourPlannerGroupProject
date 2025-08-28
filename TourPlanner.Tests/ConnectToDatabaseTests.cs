using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using SWEN2_TourPlannerGroupProject.Data;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SWEN2_TourPlannerGroupProject.Tests
{
    [TestFixture]
    public class DatabaseConnectionTests
    {
        private string _connectionString;

        [SetUp]
        public void Setup()
        {
            // Load appsettings.json from output directory
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // ensure appsettings.json is copied
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        [Test]
        public async Task CanConnectToDatabase()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(_connectionString)
                .Options;

            await using var context = new AppDbContext(options);

            try
            {
                var canConnect = await context.Database.CanConnectAsync();
                Assert.IsTrue(canConnect, "Could not connect to the database with the given connection string.");
            }
            catch (Exception ex)
            {
                Assert.Fail($"Database connection test failed: {ex.Message}");
            }
        }
    }
}
