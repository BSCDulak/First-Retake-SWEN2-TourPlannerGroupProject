using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWEN2_TourPlannerGroupProject.MVVM;
using SWEN2_TourPlannerGroupProject.Models;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace SWEN2_TourPlannerGroupProject.ViewModels
{
    /*internal class TourLogsViewModel : ViewModelBase
    {
        private TourLog _selectedTourLog;

        public TourLog SelectedTourLog
        {
            get => _selectedTourLog;
            set
            {
                SetField(ref _selectedTourLog, value);
                OnPropertyChanged(nameof(SelectedTourLog));
                CommandManager.InvalidateRequerySuggested();
            }
        }

    }*/
    internal class TourLogsViewModel : ViewModelBase
    {
        private readonly ToursListViewModel _toursListViewModel;

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
        public TourLogsViewModel() : this(new ToursListViewModel()) { }

        public TourLogsViewModel(ToursListViewModel toursListViewModel)
        {
            _toursListViewModel = toursListViewModel;
            _toursListViewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(_toursListViewModel.SelectedTour))
                {
                    OnPropertyChanged(nameof(TourLogs));
                }
            };

            AddTourLogCommand = new RelayCommand(_ => AddTourLog(), _ => _toursListViewModel.SelectedTour != null);
            DeleteTourLogCommand = new RelayCommand(_ => DeleteTourLog(), _ => SelectedTourLog != null);
        }

        private void AddTourLog()
        {
            if (_toursListViewModel.SelectedTour != null)
            {
                var newLog = new TourLog { Comment = "New Log Entry" };
                _toursListViewModel.SelectedTour.TourLogs.Add(newLog);
            }
        }

        private void DeleteTourLog()
        {
            if (_toursListViewModel.SelectedTour != null && SelectedTourLog != null)
            {
                _toursListViewModel.SelectedTour.TourLogs.Remove(SelectedTourLog);
                SelectedTourLog = null;
            }
        }
    }
}
