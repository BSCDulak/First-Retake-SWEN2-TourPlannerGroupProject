using System.Collections.ObjectModel;
using System.Windows.Input;
using SWEN2_TourPlannerGroupProject.Models;
using SWEN2_TourPlannerGroupProject.MVVM;
using System;
using System.Collections.Specialized;
using System.Linq;

namespace SWEN2_TourPlannerGroupProject.ViewModels
{
    // This class is the viewmodel for the ToursList, it contains the list of tours and the commands for adding and deleting tours.
    // this commands are triggered by the buttons in the GUI. the remove button has a condition of SelectedTour != null, which means
    // that the button is only enabled when a tour is selected in the list.
    internal class ToursListViewModel : ViewModelBase
    {
        public ObservableCollection<Tour> Tours { get; }
        private Tour? _selectedTour;

        public Tour? SelectedTour
        {
            get => _selectedTour;
            set
            {
                SetField(ref _selectedTour, value);
                OnPropertyChanged(nameof(SelectedTour));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand UpdateCalculationsCommand { get; }

        public ToursListViewModel()
        {
            // Initialize properties if necessary
        }

        public ToursListViewModel(ObservableCollection<Tour> tours)
        {
            Tours = tours;
            AddCommand = new RelayCommand(_ => AddTour());
            DeleteCommand = new RelayCommand(_ => DeleteTour(), _ => SelectedTour != null);
            UpdateCalculationsCommand = new RelayCommand(_ => UpdateAllCalculations());
            
            // Subscribe to collection changes to handle log additions/removals
            Tours.CollectionChanged += Tours_CollectionChanged;
            
            UpdateAllCalculations();
        }

        private void Tours_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                // When a new tour is added, set up event handlers for its logs
                foreach (Tour tour in e.NewItems!)
                {
                    SetupTourLogHandlers(tour);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                // When a tour is removed, clean up event handlers
                foreach (Tour tour in e.OldItems!)
                {
                    tour.TourLogs.CollectionChanged -= TourLogs_CollectionChanged;
                }
            }
        }

        private void SetupTourLogHandlers(Tour tour)
        {
            // Set default values for new tour
            tour.ChildFriendliness = "No data available";
            tour.Popularity = "No data available";
            
            // Subscribe to tour logs collection changes
            tour.TourLogs.CollectionChanged += TourLogs_CollectionChanged;
        }

        private void TourLogs_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            // Find the tour that owns these logs
            var tourLogs = sender as ObservableCollection<TourLog>;
            var tour = Tours.FirstOrDefault(t => t.TourLogs == tourLogs);
            
            if (tour != null)
            {
                // Recalculate for this specific tour
                SelectedTour = tour;
                UpdateChildFriendliness();
                UpdatePopularity();
            }
        }

        private void AddTour()
        {
            var newTour = new Tour { Name = "newTour" };
            
            // Set default values for new tour
            newTour.ChildFriendliness = "No data available";
            newTour.Popularity = "No data available";
            
            Tours.Add(newTour);
            
            // Set up event handlers for the new tour
            SetupTourLogHandlers(newTour);
        }

        private void DeleteTour()
        {
            if (SelectedTour != null)
            {
                // Clean up event handlers before removing
                SelectedTour.TourLogs.CollectionChanged -= TourLogs_CollectionChanged;
                Tours.Remove(SelectedTour);
                SelectedTour = null;
            }
        }

        public void UpdateChildFriendliness()
        {
            if (SelectedTour.TourLogs == null || SelectedTour.TourLogs.Count == 0)
            {
                SelectedTour.ChildFriendliness = "No data available"; // No data if no logs
                return;
            }

            // Calculate average difficulty
            double avgDifficulty = 0;
            int validDifficultyCount = 0;
            
            foreach (var log in SelectedTour.TourLogs)
            {
                if (double.TryParse(log.Difficulty, out double difficulty))
                {
                    avgDifficulty += difficulty;
                    validDifficultyCount++;
                }
            }

            if (validDifficultyCount > 0)
            {
                avgDifficulty /= validDifficultyCount;
            }

            // Calculate average time in hours
            double avgTimeHours = 0;
            int validTimeCount = 0;
            
            foreach (var log in SelectedTour.TourLogs)
            {
                if (log.TimeSpan != TimeSpan.Zero)
                {
                    avgTimeHours += log.TimeSpan.TotalHours;
                    validTimeCount++;
                }
            }

            if (validTimeCount > 0)
            {
                avgTimeHours /= validTimeCount;
            }

            // Calculate average distance in km
            double avgDistanceKm = 0;
            int validDistanceCount = 0;
            
            foreach (var log in SelectedTour.TourLogs)
            {
                if (log.TotalDistance > 0)
                {
                    avgDistanceKm += log.TotalDistance;
                    validDistanceCount++;
                }
            }

            if (validDistanceCount > 0)
            {
                avgDistanceKm /= validDistanceCount;
            }

            // Determine child friendliness based on criteria
            // Child friendly if: avg difficulty < 5, avg time < 3 hours, avg distance < 10 km
            if (avgDifficulty < 5 && avgTimeHours < 3 && avgDistanceKm < 10)
            {
                SelectedTour.ChildFriendliness = "Child friendly"; // Child friendly
            }
            else
            {
                SelectedTour.ChildFriendliness = "Not child friendly"; // Not child friendly
            }
        }

        public void UpdatePopularity()
        {
            if (SelectedTour.TourLogs == null || SelectedTour.TourLogs.Count == 0)
            {
                SelectedTour.Popularity = "No data available"; // No data if no logs
                return;
            }

            int logCount = SelectedTour.TourLogs.Count;

            if (logCount < 5)
            {
                SelectedTour.Popularity = "Unpopular"; // Unpopular
            }
            else if (logCount < 10)
            {
                SelectedTour.Popularity = "Popular"; // Popular
            }
            else
            {
                SelectedTour.Popularity = "Very popular"; // Very popular
            }
        }

        public void UpdateAllCalculations()
        {
            foreach (var tour in Tours)
            {
                SelectedTour = tour;
                UpdateChildFriendliness();
                UpdatePopularity();
            }
        }

    }
}
