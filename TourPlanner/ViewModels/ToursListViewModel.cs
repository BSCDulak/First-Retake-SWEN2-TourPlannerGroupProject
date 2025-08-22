using iText.Kernel.Pdf; // ✅ iText
using Microsoft.Extensions.DependencyInjection;
using SWEN2_TourPlannerGroupProject.Data;
using SWEN2_TourPlannerGroupProject.Models;
using SWEN2_TourPlannerGroupProject.MVVM;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO; // ✅ Added for file output
using System.Linq; // ✅ Added for .Any()
using System.Threading.Tasks;
using System.Windows; // ✅ For MessageBox
using System.Windows.Input;

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
        private static readonly ILoggerWrapper log = LoggerFactory.GetLogger();

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
            log.Info($"ToursListViewModel Constructor Called (Instance #{_instanceCounter})");
            
            Tours = new ObservableCollection<Tour>();
            _tourRepository = App.ServiceProvider.GetRequiredService<ITourRepository>();
            AddCommand = new RelayCommand(async _ => await AddTourAsync());
            DeleteCommand = new RelayCommand(async _ => await DeleteTourAsync(), _ => SelectedTour != null);
            UpdateCommand = new RelayCommand(async _ => await UpdateTourAsync(), _ => SelectedTour != null);
            ReportCommand = new RelayCommand(_ => GenerateTourReport(), _ => SelectedTour != null);
            UpdateCalculationsCommand = new RelayCommand(_ => UpdateAllCalculations());
            
            log.Info($"ToursListViewModel created. Tours count: {Tours.Count}");
            
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
                log.Info("Loading tours from database...");
                var tours = await _tourRepository.GetAllToursAsync();
                log.Info($"Found {tours.Count()} tours in database");

                // Ensure UI updates happen on UI thread
                App.Current.Dispatcher.Invoke(() =>
                {
                    Tours.Clear();
                    foreach (var tour in tours)
                    {
                        Tours.Add(tour);
                        log.Info($"Loaded tour: {tour.Name} from Database");
                    }
                    log.Info($"Total tours in collection: {Tours.Count}");
                    
                    // Force UI refresh
                    OnPropertyChanged(nameof(Tours));
                });
            }
            catch (Exception ex)
            {
                log.Error($"Error loading tours: {ex}");
            }
        }

        private async Task AddTourAsync()
        {
            var newTour = new Tour { Name = "newTour" };
            var addedTour = await _tourRepository.AddTourAsync(newTour);
            Tours.Add(addedTour);
            SelectedTour = addedTour;
            log.Info($"Added new tour: {addedTour.Name} with ID: {addedTour.TourId}");
            UpdateAllCalculations();
        }

        private async Task DeleteTourAsync()
        {
            if (SelectedTour != null)
            {
                log.Info($"Deleting tour: {SelectedTour.Name} with ID: {SelectedTour.TourId}");
                await _tourRepository.DeleteTourAsync(SelectedTour.TourId ?? 0);
                Tours.Remove(SelectedTour);
                SelectedTour = null;
                log.Info($"Tour deleted successfully. Remaining tours count: {Tours.Count}");
            }
        }

        private async Task UpdateTourAsync()
        {
            if (SelectedTour != null)
            {
                // Update the tour in the repository
                UpdateAllCalculations();
                await _tourRepository.UpdateTourAsync(SelectedTour);
                log.Info($"Updated tour: {SelectedTour.Name} with ID: {SelectedTour.TourId}");
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

        private void GenerateTourReport()
        {
            if (SelectedTour == null)
            {
                MessageBox.Show("Please select a tour to generate a report.", "No Tour Selected", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                var tour = SelectedTour;
                string downloadsPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads";
                string fileName = $"TourReport_{tour.Name}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                string filePath = Path.Combine(downloadsPath, fileName);

                using (var writer = new PdfWriter(filePath))
                using (var pdf = new PdfDocument(writer))
                {
                    var document = new iText.Layout.Document(pdf);

                    document.Add(new iText.Layout.Element.Paragraph("Tour Report").SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER).SetFontSize(20));
                    document.Add(new iText.Layout.Element.Paragraph($"Tour Name: {tour.Name}"));
                    document.Add(new iText.Layout.Element.Paragraph($"Description: {tour.Description}"));
                    document.Add(new iText.Layout.Element.Paragraph($"From: {tour.StartLocation}"));
                    document.Add(new iText.Layout.Element.Paragraph($"To: {tour.EndLocation}"));
                    document.Add(new iText.Layout.Element.Paragraph($"Transport Type: {tour.TransportType}"));
                    document.Add(new iText.Layout.Element.Paragraph($"Distance: {tour.Distance}"));
                    document.Add(new iText.Layout.Element.Paragraph($"Time: {tour.EstimatedTime}"));
                    document.Add(new iText.Layout.Element.Paragraph($"Child Friendliness: {tour.ChildFriendliness}"));
                    document.Add(new iText.Layout.Element.Paragraph($"Popularity: {tour.Popularity}"));

                    if (tour.TourLogs != null && tour.TourLogs.Any())
                    {
                        // --- Average Time ---
                        var validTimes = tour.TourLogs
                            .Where(log => TimeSpan.TryParse(log.TotalTime, out _))
                            .Select(log => TimeSpan.Parse(log.TotalTime).Ticks)
                            .ToList();

                        string avgTimeStr = validTimes.Any()
                            ? TimeSpan.FromTicks((long)validTimes.Average()).ToString(@"hh\:mm\:ss")
                            : "N/A";

                        // --- Average Distance ---
                        var validDistances = tour.TourLogs
                            .Where(log => double.TryParse(log.Distance, out _))
                            .Select(log => double.Parse(log.Distance))
                            .ToList();

                        string avgDistanceStr = validDistances.Any()
                            ? $"{validDistances.Average():F2} km"
                            : "N/A";

                        // --- Average Rating ---
                        var validRatings = tour.TourLogs
                            .Where(log => double.TryParse(log.Rating, out _))
                            .Select(log => double.Parse(log.Rating))
                            .ToList();

                        string avgRatingStr = validRatings.Any()
                            ? $"{validRatings.Average():F2}/5"
                            : "N/A";

                        // Add results to PDF
                        document.Add(new iText.Layout.Element.Paragraph($"Average Time: {avgTimeStr}"));
                        document.Add(new iText.Layout.Element.Paragraph($"Average Distance: {avgDistanceStr}"));
                        document.Add(new iText.Layout.Element.Paragraph($"Average Rating: {avgRatingStr}"));
                    }

                    document.Add(new iText.Layout.Element.Paragraph("Tour Logs").SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER).SetFontSize(16));

                    if (tour.TourLogs != null && tour.TourLogs.Any())
                    {
                        foreach (var log in tour.TourLogs)
                        {
                            document.Add(new iText.Layout.Element.Paragraph($"  Date: {log.Date}"));
                            document.Add(new iText.Layout.Element.Paragraph($"  Total Time: {log.TotalTime}"));
                            document.Add(new iText.Layout.Element.Paragraph($"  Report: {log.Report}"));
                            document.Add(new iText.Layout.Element.Paragraph($"  Distance: {log.Distance}"));
                            document.Add(new iText.Layout.Element.Paragraph($"  Rating: {log.Rating}"));
                            document.Add(new iText.Layout.Element.Paragraph($"  Comment: {log.Comment}"));
                            document.Add(new iText.Layout.Element.Paragraph($"  Difficulty: {log.Difficulty}"));
                            document.Add(new iText.Layout.Element.Paragraph("--------------------"));
                        }
                    }
                    else
                    {
                        document.Add(new iText.Layout.Element.Paragraph("No tour logs available for this tour."));
                    }

                    document.Close();
                }

                // Automatically open the PDF in the default browser
                System.Diagnostics.Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });

                log.Info($"Tour report generated and opened: {filePath}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating tour report: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                log.Error($"Error generating tour report: {ex}");
            }
        }
    }
}
