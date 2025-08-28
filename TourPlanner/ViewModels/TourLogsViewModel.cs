using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SWEN2_TourPlannerGroupProject.MVVM;
using SWEN2_TourPlannerGroupProject.Models;
using System.Windows.Input;
using System.Collections.ObjectModel;
using SWEN2_TourPlannerGroupProject.Data;
using Microsoft.Extensions.DependencyInjection;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System.IO;
using System.Diagnostics;


namespace SWEN2_TourPlannerGroupProject.ViewModels
{
    internal class TourLogsViewModel : ViewModelBase
    {
        private readonly ToursListViewModel _toursListViewModel;
        private readonly ITourLogRepository _tourLogRepository;
        private static readonly ILoggerWrapper log = LoggerFactory.GetLogger();

        public ObservableCollection<TourLog> TourLogs
        {
            get => _toursListViewModel.SelectedTour?.TourLogs ?? new ObservableCollection<TourLog>();
        }

        private TourLog? _selectedTourLog;
        public TourLog? SelectedTourLog
        {
            get => _selectedTourLog;
            set
            {
                SetField(ref _selectedTourLog, value);
                OnPropertyChanged(nameof(SelectedTourLog));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public ICommand AddTourLogCommand { get; }
        public ICommand DeleteTourLogCommand { get; }
        public ICommand UpdateTourLogCommand { get; }
        public ICommand ReportCommand { get; }

        public SubTabButtonsViewModel SubTabButtonsForTourLogsView { get; }

        public TourLogsViewModel() : this(new ToursListViewModel()) { }

        public TourLogsViewModel(ToursListViewModel toursListViewModel)
        {
            _toursListViewModel = toursListViewModel;
            _tourLogRepository = App.ServiceProvider.GetRequiredService<ITourLogRepository>();
            
            log.Info("TourLogsViewModel constructor called.");
            _toursListViewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(_toursListViewModel.SelectedTour))
                {
                    OnPropertyChanged(nameof(TourLogs));
                }
            };

            AddTourLogCommand = new RelayCommand(async _ => await AddTourLogAsync(), _ => _toursListViewModel.SelectedTour != null);
            DeleteTourLogCommand = new RelayCommand(async _ => await DeleteTourLogAsync(), _ => SelectedTourLog != null);
            UpdateTourLogCommand = new RelayCommand(async _ => await UpdateTourLogAsync(), _ => SelectedTourLog != null);
            ReportCommand = new RelayCommand(_ => GenerateReport(GetLogName()), _ => SelectedTourLog != null);

            SubTabButtonsForTourLogsView = new SubTabButtonsViewModel(
                AddTourLogCommand,
                DeleteTourLogCommand,
                UpdateTourLogCommand,
                ReportCommand
            );
        }

        private async Task AddTourLogAsync()
        {
            if (_toursListViewModel.SelectedTour != null)
            {
                var newLog = new TourLog 
                {
                    Name = "New Log Entry",
                    Comment = "New Log Entry",
                    TourId = _toursListViewModel.SelectedTour.TourId,
                    DateTime = DateTime.UtcNow,
                    Date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                    TotalTime = "00:00:00",
                    TimeSpan = TimeSpan.Zero
                };
                
                var addedLog = await _tourLogRepository.AddTourLogAsync(newLog);
                OnPropertyChanged(nameof(TourLogs));
                log.Info("Adding new TourLog.");
            }
            else
            {
                log.Warn("No Tour selected to add a TourLog.");
            }
        }

        private async Task DeleteTourLogAsync()
        {
            if (_toursListViewModel.SelectedTour != null && SelectedTourLog != null)
            {
                log.Info($"Deleting TourLog: ID:{SelectedTourLog.TourLogId} Name:{SelectedTourLog.Name} of Tour ID:{SelectedTourLog.TourId} Name:{SelectedTourLog.Tour?.Name}");
                if (SelectedTourLog.TourLogId.HasValue)
                {
                    await _tourLogRepository.DeleteTourLogAsync(SelectedTourLog.TourLogId.Value);
                }
                else
                {
                    log.Warn("SelectedTourLog does not have a valid TourLogId.");
                }
                SelectedTourLog = null;
                OnPropertyChanged(nameof(TourLogs));
                log.Info("TourLog deleted successfully.");
            }
        }

        private async Task UpdateTourLogAsync()
        {
            log.Info($"Updating TourLog: ID:{SelectedTourLog?.TourLogId} Name:{SelectedTourLog?.Name} of Tour ID:{SelectedTourLog?.TourId} Name:{SelectedTourLog?.Tour?.Name}");
            if (SelectedTourLog != null && SelectedTourLog.TourLogId.HasValue)
            {
                await _tourLogRepository.UpdateTourLogAsync(SelectedTourLog);
            }
        }

        private string GetLogName()
        {
            return SelectedTourLog.Name ?? "Unnamed Tour Log";
        }

        private void GenerateReport(string logName)
        {
            if (SelectedTourLog == null)
            {
                System.Diagnostics.Debug.WriteLine("No Tour Log selected for report generation.");
                return;
            }

            string downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            if (!Directory.Exists(downloadsPath))
            {
                Directory.CreateDirectory(downloadsPath);
            }
            string safeLogName = SelectedTourLog.Name ?? "New Log Entry";
            string dest = Path.Combine(downloadsPath, $"TourLogReport_{safeLogName.Replace(" ", "_")}_{DateTime.Now:yyyyMMddHHmmss}.pdf");

            PdfWriter writer = new PdfWriter(dest);
            PdfDocument pdf = new PdfDocument(writer);
            Document document = new Document(pdf);
            Paragraph header = new Paragraph(safeLogName)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(20);
            document.Add(header);

            document.Add(new Paragraph($"Comment: {SelectedTourLog.Comment ?? "N/A"}"));
            document.Add(new Paragraph($"Difficulty: {SelectedTourLog.Difficulty ?? "N/A"}"));
            document.Add(new Paragraph($"Date: {SelectedTourLog.DateTime.ToShortDateString()}"));
            document.Add(new Paragraph($"Time: {SelectedTourLog.TotalTime ?? "N/A"}"));
            document.Add(new Paragraph($"Distance: {SelectedTourLog.TotalDistance.ToString("F2")}"));
            document.Add(new Paragraph($"Rating: {SelectedTourLog.Rating ?? "N/A"}"));

            document.Close();

            System.Diagnostics.Debug.WriteLine($"Report for Tour Log: {SelectedTourLog.Name} generated at {dest}.");
            System.Diagnostics.Process.Start(new ProcessStartInfo(dest) { UseShellExecute = true });
        }

        public string Error => string.Empty;
    }
}
