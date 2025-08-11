using System.Collections.ObjectModel;
using System.Windows.Input;
using SWEN2_TourPlannerGroupProject.Models;
using SWEN2_TourPlannerGroupProject.MVVM;
using System;

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
            UpdateAllCalculations();
        }

        private void AddTour()
        {
            var newTour = new Tour { Name = "newTour" };
            Tours.Add(newTour);
        }

        private void DeleteTour()
        {
            if (SelectedTour != null)
            {
                Tours.Remove(SelectedTour);
                SelectedTour = null;
            }
        }

        public void UpdateChildFriendliness(Tour tour)
        {
            if (tour.TourLogs == null || tour.TourLogs.Count == 0)
            {
                tour.ChildFriendliness = "No data to draw from"; // No data if no logs
                return;
            }

            // Calculate average difficulty
            double avgDifficulty = 0;
            int validDifficultyCount = 0;
            
            foreach (var log in tour.TourLogs)
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
            
            foreach (var log in tour.TourLogs)
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
            
            foreach (var log in tour.TourLogs)
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
                tour.ChildFriendliness = "Child friendly"; // Child friendly
            }
            else
            {
                tour.ChildFriendliness = "Not child friendly"; // Not child friendly
            }
        }

        public void UpdatePopularity(Tour tour)
        {
            if (tour.TourLogs == null || tour.TourLogs.Count == 0)
            {
                tour.Popularity = "No data to draw from"; // No data if no logs
                return;
            }

            int logCount = tour.TourLogs.Count;

            if (logCount < 5)
            {
                tour.Popularity = "Unpopular"; // Unpopular
            }
            else if (logCount < 10)
            {
                tour.Popularity = "Popular"; // Popular
            }
            else
            {
                tour.Popularity = "Very popular"; // Very popular
            }
        }

        public void UpdateAllCalculations()
        {
            foreach (var tour in Tours)
            {
                UpdateChildFriendliness(tour);
                UpdatePopularity(tour);
            }
        }

    }
}
