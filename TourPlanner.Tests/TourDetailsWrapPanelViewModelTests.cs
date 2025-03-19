using Moq;
using NUnit.Framework;
using System.Collections.ObjectModel;
using SWEN2_TourPlannerGroupProject.ViewModels;
using SWEN2_TourPlannerGroupProject.Models;
using System.ComponentModel;

namespace SWEN2_TourPlannerGroupProject.Tests;
[TestFixture]
public class TourDetailsWrapPanelViewModelTests
{
    [Test]
    public void Integrationtest_SelectedTour_NameChange_From_TourDetailsWrapPanelViewModel_ShouldReflectInToursListViewModel()
    {
        // Arrange
        var toursList = new ObservableCollection<Tour> { new Tour { Name = "Old Tour Name" } };
        var toursListViewModel = new ToursListViewModel(toursList);
        var tourDetailsWrapPanelViewModel = new TourDetailsWrapPanelViewModel(toursListViewModel);

        // Set the SelectedTour in TourDetailsWrapPanelViewModel (this simulates the user selecting a tour)
        tourDetailsWrapPanelViewModel.SelectedTour = toursList[0];

        // Act
        // Change the name of the tour via the TourDetailsWrapPanelViewModel
        tourDetailsWrapPanelViewModel.SelectedTour.Name = "New Tour Name";

        // Assert
        // Verify that the name change is reflected in the ToursListViewModel's SelectedTour
        Assert.AreEqual("New Tour Name", toursListViewModel.SelectedTour?.Name);
    }
}