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
            /*
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

            // Add test tours with real addresses
            tours.Add(new Tour
            {
                Name = "Vienna City Tour",
                Description = "A tour through Vienna's historic city center",
                StartLocation = "Ehamgasse 8, Vienna",
                EndLocation = "Schneidergasse 4, Vienna",
                TransportType = "Walking",
                Distance = "2.5 km",
                EstimatedTime = "30 minutes",
                RouteInformation = "Historic walking tour through Vienna's city center",
                RouteImagePath = "Images/vienna_tour.png",
            });

            // Add test tours with different distances to test transport selection
            tours.Add(new Tour
            {
                Name = "Short Walk Test",
                Description = "A short walk that should be under 30 minutes",
                StartLocation = "Stephansplatz, Vienna",
                EndLocation = "Graben, Vienna",
                TransportType = "",
                Distance = "",
                EstimatedTime = "",
                RouteInformation = "This should automatically select walking",
                RouteImagePath = "Images/short_walk.png",
            });

            tours.Add(new Tour
            {
                Name = "Long Distance Test",
                Description = "A longer route that should require public transport or car",
                StartLocation = "Vienna International Airport",
                EndLocation = "Stephansplatz, Vienna",
                TransportType = "",
                Distance = "",
                EstimatedTime = "",
                RouteInformation = "This should automatically select public transport or car",
                RouteImagePath = "Images/long_distance.png",
            });

            tours.Add(new Tour
            {
                Name = "Invalid Route Test",
                Description = "This tour has invalid locations to test fallback",
                StartLocation = "Invalid Address 123",
                EndLocation = "NonExistent Street 456",
                TransportType = "Car",
                Distance = "Unknown",
                EstimatedTime = "Unknown",
                RouteInformation = "This should show the default Vienna map",
                RouteImagePath = "Images/fallback.png",
            });

            tours.Add(new Tour
            {
                Name = "Incomplete Route Test",
                Description = "This tour has only one location to test fallback",
                StartLocation = "Ehamgasse 8, Vienna",
                EndLocation = "", // Empty end location
                TransportType = "Walking",
                Distance = "Unknown",
                EstimatedTime = "Unknown",
                RouteInformation = "This should show the default Vienna map due to missing end location",
                RouteImagePath = "Images/incomplete.png",
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
                Difficulty = "5",
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
                Difficulty = "5",
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
                Difficulty = "5",
                Report = "Some rain but manageable",
                Name = "Log2"
            });

            // Vienna City Tour logs
            var viennaTour = tours.First(t => t.Name == "Vienna City Tour");
            viennaTour.TourLogs.Add(new TourLog
            {
                Date = "10.08.2025",
                TotalTime = "1:45:32",
                TotalDistance = 4.5,
                Rating = "4",
                AverageSpeed = "2.57",
                Comment = "Beautiful scenery throughout the route",
                Difficulty = "5",
                Report = "Sunny with a light breeze",
                Name = "Log 1"
            });
            viennaTour.TourLogs.Add(new TourLog
            {
                Date = "08.08.2025",
                TotalTime = "3:10:15",
                TotalDistance = 8.2,
                Rating = "5",
                AverageSpeed = "2.59",
                Comment = "Challenging climb but worth it",
                Difficulty = "8",
                Report = "Overcast but comfortable",
                Name = "Log 2"
            });
            viennaTour.TourLogs.Add(new TourLog
            {
                Date = "05.08.2025",
                TotalTime = "0:45:00",
                TotalDistance = 2.1,
                Rating = "3",
                AverageSpeed = "2.80",
                Comment = "Easy and relaxing walk",
                Difficulty = "2",
                Report = "Clear skies and warm",
                Name = "Log 3"
            });
            viennaTour.TourLogs.Add(new TourLog
            {
                Date = "02.08.2025",
                TotalTime = "5:20:42",
                TotalDistance = 11.3,
                Rating = "4",
                AverageSpeed = "2.11",
                Comment = "Lots of photo opportunities",
                Difficulty = "7",
                Report = "Windy at higher elevations",
                Name = "Log 4"
            });

            // Short Walk Test logs
            var shortWalk = tours.First(t => t.Name == "Short Walk Test");
            shortWalk.TourLogs.Add(new TourLog
            {
                Date = "10.08.2025",
                TotalTime = "0:25:15",
                TotalDistance = 1.9,
                Rating = "4",
                AverageSpeed = "4.52",
                Comment = "Good for beginners",
                Difficulty = "1",
                Report = "Sunny with a light breeze",
                Name = "Log 1"
            });
            shortWalk.TourLogs.Add(new TourLog
            {
                Date = "09.08.2025",
                TotalTime = "0:40:20",
                TotalDistance = 3.0,
                Rating = "5",
                AverageSpeed = "4.47",
                Comment = "Easy and relaxing walk",
                Difficulty = "2",
                Report = "Overcast but comfortable",
                Name = "Log 2"
            });
            shortWalk.TourLogs.Add(new TourLog
            {
                Date = "08.08.2025",
                TotalTime = "0:32:45",
                TotalDistance = 2.2,
                Rating = "4",
                AverageSpeed = "4.03",
                Comment = "Lots of photo opportunities",
                Difficulty = "3",
                Report = "Clear skies and warm",
                Name = "Log 3"
            });
            shortWalk.TourLogs.Add(new TourLog
            {
                Date = "06.08.2025",
                TotalTime = "1:10:00",
                TotalDistance = 5.0,
                Rating = "3",
                AverageSpeed = "4.29",
                Comment = "Some traffic noise in parts",
                Difficulty = "5",
                Report = "Windy at higher elevations",
                Name = "Log 4"
            });
            shortWalk.TourLogs.Add(new TourLog
            {
                Date = "04.08.2025",
                TotalTime = "0:18:30",
                TotalDistance = 1.8,
                Rating = "4",
                AverageSpeed = "5.84",
                Comment = "Would definitely do again",
                Difficulty = "4",
                Report = "Crowded in popular areas",
                Name = "Log 5"
            });
            shortWalk.TourLogs.Add(new TourLog
            {
                Date = "02.08.2025",
                TotalTime = "0:55:12",
                TotalDistance = 4.4,
                Rating = "3",
                AverageSpeed = "4.79",
                Comment = "Good for beginners",
                Difficulty = "6",
                Report = "Peaceful and quiet",
                Name = "Log 6"
            });
            shortWalk.TourLogs.Add(new TourLog
            {
                Date = "30.07.2025",
                TotalTime = "0:20:45",
                TotalDistance = 2.0,
                Rating = "5",
                AverageSpeed = "5.78",
                Comment = "Weather was perfect",
                Difficulty = "2",
                Report = "Sunny with a light breeze",
                Name = "Log 7"
            });
            shortWalk.TourLogs.Add(new TourLog
            {
                Date = "28.07.2025",
                TotalTime = "0:50:30",
                TotalDistance = 4.1,
                Rating = "4",
                AverageSpeed = "4.88",
                Comment = "Some traffic noise in parts",
                Difficulty = "5",
                Report = "Overcast but comfortable",
                Name = "Log 8"
            });

            // Long Distance Test logs
            var longDistance = tours.First(t => t.Name == "Long Distance Test");
            longDistance.TourLogs.Add(new TourLog
            {
                Date = "10.08.2025",
                TotalTime = "4:15:10",
                TotalDistance = 9.8,
                Rating = "5",
                AverageSpeed = "2.30",
                Comment = "Challenging climb but worth it",
                Difficulty = "9",
                Report = "Sunny with a light breeze",
                Name = "Log 1"
            });
            longDistance.TourLogs.Add(new TourLog
            {
                Date = "08.08.2025",
                TotalTime = "6:50:25",
                TotalDistance = 12.0,
                Rating = "4",
                AverageSpeed = "1.75",
                Comment = "Some traffic noise in parts",
                Difficulty = "10",
                Report = "Overcast but comfortable",
                Name = "Log 2"
            });
            longDistance.TourLogs.Add(new TourLog
            {
                Date = "06.08.2025",
                TotalTime = "3:35:00",
                TotalDistance = 10.2,
                Rating = "3",
                AverageSpeed = "2.84",
                Comment = "Good for beginners",
                Difficulty = "6",
                Report = "Clear skies and warm",
                Name = "Log 3"
            });
            longDistance.TourLogs.Add(new TourLog
            {
                Date = "04.08.2025",
                TotalTime = "7:10:40",
                TotalDistance = 11.7,
                Rating = "5",
                AverageSpeed = "1.63",
                Comment = "Beautiful scenery throughout the route",
                Difficulty = "8",
                Report = "Windy at higher elevations",
                Name = "Log 4"
            });
            longDistance.TourLogs.Add(new TourLog
            {
                Date = "02.08.2025",
                TotalTime = "2:20:15",
                TotalDistance = 7.4,
                Rating = "4",
                AverageSpeed = "3.17",
                Comment = "Lots of photo opportunities",
                Difficulty = "5",
                Report = "Crowded in popular areas",
                Name = "Log 5"
            });
            longDistance.TourLogs.Add(new TourLog
            {
                Date = "30.07.2025",
                TotalTime = "5:05:05",
                TotalDistance = 9.5,
                Rating = "3",
                AverageSpeed = "1.87",
                Comment = "Some construction along the way",
                Difficulty = "7",
                Report = "Peaceful and quiet",
                Name = "Log 6"
            });
            longDistance.TourLogs.Add(new TourLog
            {
                Date = "28.07.2025",
                TotalTime = "1:25:55",
                TotalDistance = 5.1,
                Rating = "4",
                AverageSpeed = "3.56",
                Comment = "Would definitely do again",
                Difficulty = "4",
                Report = "Sunny with a light breeze",
                Name = "Log 7"
            });
            longDistance.TourLogs.Add(new TourLog
            {
                Date = "26.07.2025",
                TotalTime = "0:50:20",
                TotalDistance = 3.4,
                Rating = "5",
                AverageSpeed = "4.05",
                Comment = "Weather was perfect",
                Difficulty = "3",
                Report = "Overcast but comfortable",
                Name = "Log 8"
            });
            */
            //this makes a new TourListViewModel and binds it to the tours list
            ToursListView = new ToursListViewModel();
            TourLogs = new TourLogsViewModel(ToursListView);
            //todo create a TourListTourLogsViewModel -> this makes a new TourListViewModel and binds it to the tour logs list
            //ToursListTourLogsViewModel = new ToursListView(not sure if we gotta do tours or tourlogs here);
            // this binds the subtab buttons to the tours viewmodel
            SubTabButtonsForToursListView = new SubTabButtonsViewModel(
                ToursListView.AddCommand,
                ToursListView.DeleteCommand,
                ToursListView.UpdateCommand
            );
            // this binds the subtab buttons to the tour logs viewmodel
            SubTabButtonsForTourLogsView = new SubTabButtonsViewModel(
                TourLogs.AddTourLogCommand,
                TourLogs.DeleteTourLogCommand
            );
            TourDetailsWrapPanelView = new TourDetailsWrapPanelViewModel(ToursListView);
            MapViewModel = new MapViewModel();

            // Connect the selected tour to the map view model
            ToursListView.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(ToursListViewModel.SelectedTour))
                {
                    MapViewModel.SetSelectedTour(ToursListView.SelectedTour);
                }
            };
        }
    }
}