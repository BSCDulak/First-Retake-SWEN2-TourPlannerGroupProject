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
        }

        private async Task AddTourLogAsync()
        {
            if (_toursListViewModel.SelectedTour != null)
            {
                var newLog = new TourLog 
                { 
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
            else
            {
                log.Warn("SelectedTourLog is null or does not have a valid TourLogId.");
            }
        }



        public string Error => null;
    }
}
