using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using SWEN2_TourPlannerGroupProject.Data;
using SWEN2_TourPlannerGroupProject.Models;
using SWEN2_TourPlannerGroupProject.ViewModels;
using SWEN2_TourPlannerGroupProject.MVVM;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWEN2_TourPlannerGroupProject.Tests
{
    [TestFixture]
    public class TourLogsViewModelTests
    {
        private ITourRepository _tourRepository = null!;
        private ITourLogRepository _tourLogRepository = null!;

        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("TestDatabase" + System.Guid.NewGuid()));
            services.AddScoped<ITourRepository, TourRepository>();
            services.AddScoped<ITourLogRepository, TourLogRepository>();

            App.ConfigureServicesForTest(services.BuildServiceProvider());
            _tourRepository = App.ServiceProvider.GetRequiredService<ITourRepository>();
            _tourLogRepository = App.ServiceProvider.GetRequiredService<ITourLogRepository>();
        }

        [Test]
        public async Task AddTourLog_ShouldIncreaseTourLogsCount()
        {
            var toursVm = new ToursListViewModel();
            await toursVm.InitializeAsync();

            var tour = await _tourRepository.AddTourAsync(new Tour { Name = "Tour 1" });
            await toursVm.InitializeAsync();
            // Select the newly added tour to simulate user selection in UI
            toursVm.SelectedTour = tour;

            var tourLogsVm = new TourLogsViewModel(toursVm);

            await ((AsyncRelayCommand)tourLogsVm.AddTourLogCommand).ExecuteAsync();

            Assert.That(toursVm.SelectedTour!.TourLogs.Count, Is.EqualTo(1));
            Assert.That(toursVm.SelectedTour.TourLogs.First().Name, Is.EqualTo("New Log Entry"));
        }

        [Test]
        public async Task DeleteTourLog_ShouldRemoveSingleTourLog()
        {
            var toursVm = new ToursListViewModel();
            await toursVm.InitializeAsync();

            var tour = await _tourRepository.AddTourAsync(new Tour { Name = "Tour 1" });
            var log1 = await _tourLogRepository.AddTourLogAsync(new TourLog { Name = "Log 1", TourId = tour.TourId });
            await toursVm.InitializeAsync();
            // Select the newly added tour to simulate user selection in UI
            toursVm.SelectedTour = tour;
            var tourLogsVm = new TourLogsViewModel(toursVm);
            tourLogsVm.SelectedTourLogs = new List<TourLog> { tour.TourLogs[0] };
            tourLogsVm.SelectedTourLog = tour.TourLogs[0];

            await ((AsyncRelayCommand)tourLogsVm.DeleteTourLogCommand).ExecuteAsync();

            Assert.That(tour.TourLogs.Count, Is.EqualTo(0));
            CollectionAssert.IsEmpty(tourLogsVm.SelectedTourLogs);

            var remainingLogs = await _tourLogRepository.GetTourLogsByTourIdAsync(tour.TourId!.Value);
            CollectionAssert.IsEmpty(remainingLogs);
        }

        [Test]
        public async Task DeleteMultipleTourLogs_ShouldRemoveAllSelectedTourLogs()
        {
            var toursVm = new ToursListViewModel();
            await toursVm.InitializeAsync();

            var tour = await _tourRepository.AddTourAsync(new Tour { Name = "Tour 1" });
            await _tourLogRepository.AddTourLogAsync(new TourLog { Name = "Log 1", TourId = tour.TourId });
            await _tourLogRepository.AddTourLogAsync(new TourLog { Name = "Log 2", TourId = tour.TourId });
            await _tourLogRepository.AddTourLogAsync(new TourLog { Name = "Log 3", TourId = tour.TourId });
            await toursVm.InitializeAsync();
            // Select the newly added tour to simulate user selection in UI
            toursVm.SelectedTour = tour;
            var tourLogsVm = new TourLogsViewModel(toursVm);

            // Select first and last logs
            tourLogsVm.SelectedTourLogs = new List<TourLog> { tour.TourLogs[0], tour.TourLogs[2] };
            tourLogsVm.SelectedTourLog = tour.TourLogs[0];

            await ((AsyncRelayCommand)tourLogsVm.DeleteTourLogCommand).ExecuteAsync();

            Assert.That(tour.TourLogs.Count, Is.EqualTo(1));
            Assert.That(tour.TourLogs.First().Name, Is.EqualTo("Log 2"));
            CollectionAssert.IsEmpty(tourLogsVm.SelectedTourLogs);

            var remainingLogs = (await _tourLogRepository.GetTourLogsByTourIdAsync(tour.TourId!.Value))
                                .Select(l => l.Name)
                                .ToList();
            CollectionAssert.AreEqual(new[] { "Log 2" }, remainingLogs);
        }

        [Test]
        public async Task UpdateTourLog_ShouldPersistChanges()
        {
            var toursVm = new ToursListViewModel();
            await toursVm.InitializeAsync();

            var tour = await _tourRepository.AddTourAsync(new Tour { Name = "Tour 1" });
            var log1 = await _tourLogRepository.AddTourLogAsync(new TourLog { Name = "Log 1", TourId = tour.TourId });
            await toursVm.InitializeAsync();

            var tourLogsVm = new TourLogsViewModel(toursVm);
            tourLogsVm.SelectedTourLog = tour.TourLogs[0];

            // Update property
            tourLogsVm.SelectedTourLog.Comment = "Updated Comment";
            await ((AsyncRelayCommand)tourLogsVm.UpdateTourLogCommand).ExecuteAsync();

            // Safely unwrap nullable ID for repository
            var logId = tour.TourLogs[0].TourLogId;
            Assert.That(logId.HasValue, Is.True, "TourLogId should have a value");

            var updatedLog = await _tourLogRepository.GetTourLogByIdAsync(logId.Value);
            Assert.That(updatedLog!.Comment, Is.EqualTo("Updated Comment"));
        }
    }
}
