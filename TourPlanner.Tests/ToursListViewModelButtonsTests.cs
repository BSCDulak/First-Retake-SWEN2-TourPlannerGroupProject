using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using SWEN2_TourPlannerGroupProject.Data;
using SWEN2_TourPlannerGroupProject.Models;
using SWEN2_TourPlannerGroupProject.ViewModels;
using SWEN2_TourPlannerGroupProject.MVVM;

using System.Linq;
using System.Threading.Tasks;

namespace SWEN2_TourPlannerGroupProject.Tests
{
    [TestFixture]
    public class ToursListViewModelTests
    {
        private ITourRepository _tourRepository = null!;

        [SetUp]
        public void Setup()
        {
            // Configure DI with InMemory DB for testing
            var services = new ServiceCollection();
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("TestDatabase" + System.Guid.NewGuid().ToString()));
            services.AddScoped<ITourRepository, TourRepository>();
            services.AddScoped<ITourLogRepository, TourLogRepository>();

            App.ConfigureServicesForTest(services.BuildServiceProvider());
            _tourRepository = App.ServiceProvider.GetRequiredService<ITourRepository>();
        }

        [Test]
        public async Task AddTour_ShouldIncreaseTourCount()
        {
            var vm = new ToursListViewModel();
            await vm.InitializeAsync();

            // AsyncRelayCommand: ExecuteAsync extension method
            await ((AsyncRelayCommand)vm.AddCommand).ExecuteAsync();

            Assert.That(vm.Tours.Count, Is.EqualTo(1));
            Assert.That(vm.SelectedTour, Is.Not.Null);
            Assert.That(vm.SelectedTour!.Name, Is.EqualTo("newTour"));
        }

        [Test]
        public async Task DeleteTour_ShouldRemoveTourFromList()
        {
            var vm = new ToursListViewModel();
            await vm.InitializeAsync();

            await ((AsyncRelayCommand)vm.AddCommand).ExecuteAsync();
            var tourToDelete = vm.Tours.First();
            vm.SelectedTour = tourToDelete;

            await ((AsyncRelayCommand)vm.DeleteCommand).ExecuteAsync();

            Assert.That(vm.Tours.Count, Is.EqualTo(0));
            Assert.That(vm.SelectedTour, Is.Null);
        }

        [Test]
        public async Task UpdateTour_ShouldPersistChanges()
        {
            var vm = new ToursListViewModel();
            await vm.InitializeAsync();

            await ((AsyncRelayCommand)vm.AddCommand).ExecuteAsync();
            var tourToUpdate = vm.Tours.First();
            vm.SelectedTour = tourToUpdate;

            vm.SelectedTour.Name = "UpdatedName";
            await ((AsyncRelayCommand)vm.UpdateCommand).ExecuteAsync();

            var allTours = await _tourRepository.GetAllToursAsync();
            Assert.That(allTours.First().Name, Is.EqualTo("UpdatedName"));
        }

        [Test]
        public async Task LoadTours_ShouldPopulateToursCollection()
        {
            // Arrange: seed repository directly
            var tour1 = new Tour { Name = "Seeded Tour 1" };
            var tour2 = new Tour { Name = "Seeded Tour 2" };
            await _tourRepository.AddTourAsync(tour1);
            await _tourRepository.AddTourAsync(tour2);

            // Act
            var vm = new ToursListViewModel();
            await vm.InitializeAsync(); // now loads tours from the repo

            // Assert
            Assert.That(vm.Tours.Count, Is.EqualTo(2));
            CollectionAssert.AreEquivalent(
                new[] { "Seeded Tour 1", "Seeded Tour 2" },
                vm.Tours.Select(t => t.Name).ToList()
            );
        }
        [Test]
        public async Task DeleteMultipleTours_ShouldRemoveAllSelectedTours()
        {
            // Arrange
            var vm = new ToursListViewModel();
            await vm.InitializeAsync();

            var tour1 = await _tourRepository.AddTourAsync(new Tour { Name = "Tour 1" });
            var tour2 = await _tourRepository.AddTourAsync(new Tour { Name = "Tour 2" });
            var tour3 = await _tourRepository.AddTourAsync(new Tour { Name = "Tour 3" });

            await vm.InitializeAsync(); // reload tours
            vm.SelectedTours = new List<Tour> { vm.Tours[0], vm.Tours[2] }; // select first and last
            vm.SelectedTour = vm.Tours[2]; // to simulate User clicking on last item because SelectedTour is used in DeleteCommand CanExecute
            // Act
            await ((AsyncRelayCommand)vm.DeleteCommand).ExecuteAsync();

            // Assert
            Assert.That(vm.Tours.Count, Is.EqualTo(1));
            Assert.That(vm.Tours.First().Name, Is.EqualTo("Tour 2"));
            CollectionAssert.IsEmpty(vm.SelectedTours);

            var remainingTours = (await _tourRepository.GetAllToursAsync()).Select(t => t.Name).ToList();
            CollectionAssert.AreEqual(new[] { "Tour 2" }, remainingTours);
        }
    }
}