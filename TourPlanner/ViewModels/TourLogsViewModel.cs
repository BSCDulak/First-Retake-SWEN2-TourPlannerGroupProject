using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                _toursListViewModel.SelectedTour.TourLogs.Add(addedLog);
                OnPropertyChanged(nameof(TourLogs));
            }
        }

        private async Task DeleteTourLogAsync()
        {
            if (_toursListViewModel.SelectedTour != null && SelectedTourLog != null)
            {
                if (SelectedTourLog.TourLogId.HasValue)
                {
                    await _tourLogRepository.DeleteTourLogAsync(SelectedTourLog.TourLogId.Value);
                }
                _toursListViewModel.SelectedTour.TourLogs.Remove(SelectedTourLog);
                SelectedTourLog = null;
                OnPropertyChanged(nameof(TourLogs));
            }
        }

        private async Task UpdateTourLogAsync()
        {
            if (SelectedTourLog != null && SelectedTourLog.TourLogId.HasValue)
            {
                await _tourLogRepository.UpdateTourLogAsync(SelectedTourLog);
            }
        }
    }
}
