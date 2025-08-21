using System.Collections.ObjectModel;
using System.Windows.Input;
using SWEN2_TourPlannerGroupProject.Models;
using SWEN2_TourPlannerGroupProject.MVVM;
using SWEN2_TourPlannerGroupProject.Data;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using System.IO; // ✅ Added for file output
using iText.Kernel.Pdf; // ✅ iText
using System.Windows; // ✅ For MessageBox

namespace SWEN2_TourPlannerGroupProject.ViewModels
{
    // This class is the viewmodel for the ToursList, it contains the list of tours and the commands for adding and deleting tours.
    // this commands are triggered by the buttons in the GUI. the remove button has a condition of SelectedTour != null, which means
    // that the button is only enabled when a tour is selected in the list.
    internal class ToursListViewModel : ViewModelBase
    {
        private static int _instanceCounter = 0;
        private readonly ITourRepository _tourRepository;
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
        public ICommand UpdateCommand { get; }
        public ICommand UpdateCalculationsCommand { get; }
        public ICommand ReportCommand { get; } 
        public ToursListViewModel()
        {
            _instanceCounter++;
            System.Diagnostics.Debug.WriteLine($"ToursListViewModel Constructor Called (Instance #{_instanceCounter})");
            
            Tours = new ObservableCollection<Tour>();
            _tourRepository = App.ServiceProvider.GetRequiredService<ITourRepository>();
            AddCommand = new RelayCommand(async _ => await AddTourAsync());
            DeleteCommand = new RelayCommand(async _ => await DeleteTourAsync(), _ => SelectedTour != null);
            UpdateCommand = new RelayCommand(async _ => await UpdateTourAsync(), _ => SelectedTour != null);
            ReportCommand = new RelayCommand(_ => { /* Mock report action */ });
            UpdateCalculationsCommand = new RelayCommand(_ => UpdateAllCalculations());
            
            System.Diagnostics.Debug.WriteLine($"ToursListViewModel created. Tours count: {Tours.Count}");
            
            // Load data asynchronously to avoid blocking the UI thread
            _ = Task.Run(async () =>
            {
                await LoadToursAsync();
                App.Current.Dispatcher.Invoke(() => UpdateAllCalculations());
            });
        }

        private async Task LoadToursAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Loading tours from database...");
                var tours = await _tourRepository.GetAllToursAsync();
                System.Diagnostics.Debug.WriteLine($"Found {tours.Count()} tours in database");

                // Ensure UI updates happen on UI thread
                App.Current.Dispatcher.Invoke(() =>
                {
                    Tours.Clear();
                    foreach (var tour in tours)
                    {
                        Tours.Add(tour);
                        System.Diagnostics.Debug.WriteLine($"Added tour: {tour.Name}");
                    }
                    System.Diagnostics.Debug.WriteLine($"Total tours in collection: {Tours.Count}");
                    
                    // Force UI refresh
                    OnPropertyChanged(nameof(Tours));
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading tours: {ex.Message}");
            }
        }

        private async Task AddTourAsync()
        {
            var newTour = new Tour { Name = "newTour" };
            var addedTour = await _tourRepository.AddTourAsync(newTour);
            Tours.Add(addedTour);
            SelectedTour = addedTour;
            UpdateAllCalculations();
        }

        private async Task DeleteTourAsync()
        {
            if (SelectedTour != null)
            {
                await _tourRepository.DeleteTourAsync(SelectedTour.TourId ?? 0);
                Tours.Remove(SelectedTour);
                SelectedTour = null;
            }
        }

        private async Task UpdateTourAsync()
        {
            if (SelectedTour != null)
            {
                // Update the tour in the repository
                UpdateAllCalculations();
                await _tourRepository.UpdateTourAsync(SelectedTour);
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

        // This method updates all calculations for each tour in the Tours collection. Possible concerns: Might be slow for large datasets causing UI lag.
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
