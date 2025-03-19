
using NUnit.Framework;
using SWEN2_TourPlannerGroupProject.ViewModels;
using SWEN2_TourPlannerGroupProject.Models;
using System.Collections.ObjectModel;

namespace SWEN2_TourPlannerGroupProject.Tests;
[TestFixture]
public class ToursListViewModelButtonsTests
{
    private ToursListViewModel _viewModel;

    [SetUp] // This method runs before each test to initialize your test setup
    public void SetUp()
    {
        // Initialize the view model with an empty ObservableCollection
        _viewModel = new ToursListViewModel(new ObservableCollection<Tour>());
    }

    [Test] // This attribute marks the method as a test method
    public void AddTour_ShouldAddNewTourToCollection()
    {
        // Arrange
        int initialCount = _viewModel.Tours.Count;

        // Act
        _viewModel.AddCommand.Execute(null);

        // Assert
        Assert.AreEqual(initialCount + 1, _viewModel.Tours.Count);
        Assert.AreEqual("newTour", _viewModel.Tours[initialCount].Name);
    }

    [Test]
    public void DeleteTour_ShouldRemoveSelectedTour()
    {
        // Arrange
        // Manually create and populate the collection with a sample tour, because if we use .Add we depend on
        // the Add method working correctly which turns this into an integration test -> and that is not a unit test
        var tourList = new ObservableCollection<Tour>
        {
            new Tour { Name = "Test Tour" }
        };

        // Initialize ViewModel with the predefined list
        _viewModel = new ToursListViewModel(tourList);

        // Select the tour to delete
        var selectedTour = tourList[0];
        _viewModel.SelectedTour = selectedTour;

        int initialCount = _viewModel.Tours.Count;

        // Act
        _viewModel.DeleteCommand.Execute(null);

        // Assert
        Assert.AreEqual(initialCount - 1, _viewModel.Tours.Count);  // Ensure the count is decremented
        Assert.IsNull(_viewModel.SelectedTour);  // Ensure the selected tour is null after deletion
    }

    [Test]
    public void DeleteTour_ShouldNotExecuteWhenNoTourIsSelected()
    {
        // Arrange
        _viewModel.SelectedTour = null;
        int initialCount = _viewModel.Tours.Count;

        // Act
        _viewModel.DeleteCommand.Execute(null);

        // Assert
        Assert.AreEqual(initialCount, _viewModel.Tours.Count, "Cannot delete a tour if you haven't selected one");  // The collection should remain the same
    }
    [Test]
    public void SelectedTour_ShouldBeActuallySelected()
    {
        // Arrange
        var toursListViewModel = new ToursListViewModel(new ObservableCollection<Tour>());
        

        var tour = new Tour { Name = "Test Tour" };

        // Act

        toursListViewModel.SelectedTour = tour;
        var selectedTourFromToursListViewModel = toursListViewModel.SelectedTour;

        // Assert
        Assert.AreEqual(tour, selectedTourFromToursListViewModel);
    }
}