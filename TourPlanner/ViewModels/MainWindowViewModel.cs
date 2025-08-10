using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWEN2_TourPlannerGroupProject.Models;

namespace SWEN2_TourPlannerGroupProject.ViewModels
{
    // This is the main viewmodel for the MainWindow, it contains the viewmodels for
    // the ToursList, SubTabButtons and TourDetailsWrapPanel.
    // every time a new viewmodel is created it needs to be added here and initialized in the constructor.
    internal class MainWindowViewModel : ViewModelBase
    {
        public ToursListViewModel ToursListView { get; }
        public TourLogsViewModel TourLogs { get; }
        public SubTabButtonsViewModel SubTabButtonsForToursListView { get; }
        public SubTabButtonsViewModel SubTabButtonsForTourLogsView { get; }
        public TourDetailsWrapPanelViewModel TourDetailsWrapPanelView { get; }
        public MapViewModel MapViewModel { get; }

        public MainWindowViewModel()
        {
            var tours = new ObservableCollection<Tour>();
            for (var i = 1; i < 10; i++)
            {
                tours.Add(new Tour
                {
                    Name = $"Tour{i}"
                });
            }

            tours.Add(new Tour
            {
                Name = "Tour10",
                Description = "This is a test tour with a very long description to test the wrapping of the text in the TourDetailsWrapPanel. " +
                              "It should be long enough to ensure that the text wraps correctly and does not overflow the panel.",
                StartLocation = "Start Location",
                EndLocation = "End Location",
                TransportType = "Car",
                Distance = "100 km",
                EstimatedTime = "1 hour",
                RouteInformation = "This is a test route information for the TourDetailsWrapPanel. It should be long enough to ensure that the text wraps correctly and does not overflow the panel.",
                RouteImagePath = "Images/route_image.png",
            });

            // Add test data for TourLogs
            var tour1 = tours[0];
            tour1.TourLogs.Add(new TourLog 
            { 
                Date = DateTime.Now.ToString("dd.MM.yyyy"),
                TotalTime = "2:00:00",
                TotalDistance = 10.5,
                Rating = "4",
                AverageSpeed = "5.25",
                Comment = "Great tour with beautiful scenery",
                Difficulty = "Moderate",
                Report = "Perfect weather conditions",
                Name = "First Log"
            });

            tour1.TourLogs.Add(new TourLog
            {
                Date = DateTime.Now.ToString("dd.MM.yyyy"),
                TotalTime = "1:00:00",
                TotalDistance = 10.5,
                Rating = "1",
                AverageSpeed = "5.25",
                Comment = "Ez Tour",
                Difficulty = "Easy",
                Report = "Perfect weather conditions",
                Name = "Second Log"
            });

            var tour2 = tours[1];
            tour2.TourLogs.Add(new TourLog 
            { 
                Date = DateTime.Now.AddDays(-1).ToString("dd.MM.yyyy"),
                TotalTime = "3:00:00",
                TotalDistance = 15.0,
                Rating = "5",
                AverageSpeed = "5.0",
                Comment = "Challenging but rewarding hike",
                Difficulty = "Hard",
                Report = "Some rain but manageable",
                Name = "Log2"
            });

            //this makes a new TourListViewModel and binds it to the tours list
            ToursListView = new ToursListViewModel(tours);
            TourLogs = new TourLogsViewModel(ToursListView);
            //todo create a TourListTourLogsViewModel -> this makes a new TourListViewModel and binds it to the tour logs list
            //ToursListTourLogsViewModel = new ToursListView(not sure if we gotta do tours or tourlogs here);
            // this binds the subtab buttons to the tours viewmodel
            SubTabButtonsForToursListView = new SubTabButtonsViewModel(
                ToursListView.AddCommand,
                ToursListView.DeleteCommand
            );
            // this binds the subtab buttons to the tour logs viewmodel
            SubTabButtonsForTourLogsView = new SubTabButtonsViewModel(
                TourLogs.AddTourLogCommand,
                TourLogs.DeleteTourLogCommand
            );
            TourDetailsWrapPanelView = new TourDetailsWrapPanelViewModel(ToursListView);
            MapViewModel = new MapViewModel();
        }
    }
}