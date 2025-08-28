using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using SWEN2_TourPlannerGroupProject.Data;
using SWEN2_TourPlannerGroupProject.Models;
using SWEN2_TourPlannerGroupProject.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWEN2_TourPlannerGroupProject.Tests
{
    [TestFixture]
    public class ToursListViewModelTests
    {
        [SetUp]
        public void Setup()
        {
            // Configure DI with InMemory DB for testing
            var services = new ServiceCollection();
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("TestDatabase" + System.Guid.NewGuid().ToString()));
            // unique name ensures a fresh DB each test

            services.AddScoped<ITourRepository, TourRepository>();
            services.AddScoped<ITourLogRepository, TourLogRepository>();

            App.ConfigureServicesForTest(services.BuildServiceProvider());
        }

        [Test]
        public async Task AddTour_ShouldIncreaseTourCount()
        {
            var vm = new ToursListViewModel();
            await Task.Run(() => vm.AddCommand.Execute(null));

            Assert.That(vm.Tours.Count, Is.EqualTo(1));
            Assert.That(vm.SelectedTour, Is.Not.Null);
            Assert.That(vm.SelectedTour.Name, Is.EqualTo("newTour"));
        }

        [Test]
        public async Task DeleteTour_ShouldRemoveTourFromList()
        {
            var vm = new ToursListViewModel();
            await Task.Run(() => vm.AddCommand.Execute(null));
            var tourToDelete = vm.Tours.First();
            vm.SelectedTour = tourToDelete;

            await Task.Run(() => vm.DeleteCommand.Execute(null));

            Assert.That(vm.Tours.Count, Is.EqualTo(0));
            Assert.That(vm.SelectedTour, Is.Null);
        }

        [Test]
        public async Task UpdateTour_ShouldPersistChanges()
        {
            var vm = new ToursListViewModel();
            await Task.Run(() => vm.AddCommand.Execute(null));
            var tourToUpdate = vm.Tours.First();
            vm.SelectedTour = tourToUpdate;

            vm.SelectedTour.Name = "UpdatedName";
            await Task.Run(() => vm.UpdateCommand.Execute(null));

            var repo = App.ServiceProvider.GetRequiredService<ITourRepository>();
            var allTours = await repo.GetAllToursAsync();

            Assert.That(allTours.First().Name, Is.EqualTo("UpdatedName"));
        }
    }
}
