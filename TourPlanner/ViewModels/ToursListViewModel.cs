using System.Collections.ObjectModel;
using System.Windows.Input;
using SWEN2_TourPlannerGroupProject.Models;
using SWEN2_TourPlannerGroupProject.MVVM;
using SWEN2_TourPlannerGroupProject.Services;
using System.Windows;

namespace SWEN2_TourPlannerGroupProject.ViewModels
{
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

        private string _newTourName;
        public string NewTourName
        {
            get => _newTourName;
            set
            {
                if (SetField(ref _newTourName, value))
                {
                    CommandManager.InvalidateRequerySuggested(); 
                }
            }
        }

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }

        private readonly TourDataService _dataService;

        
        public ToursListViewModel(ObservableCollection<Tour> tours)
        {
            Tours = tours;
            _dataService = new TourDataService();

            AddCommand = new RelayCommand(_ => AddTour(), _ => !string.IsNullOrWhiteSpace(NewTourName));
            DeleteCommand = new RelayCommand(_ => DeleteTour(), _ => SelectedTour != null);
        }

        
        public ToursListViewModel()
        {
            Tours = new ObservableCollection<Tour>();
            AddCommand = new RelayCommand(_ => { });
            DeleteCommand = new RelayCommand(_ => { });
            _dataService = new TourDataService();
        }

        private void AddTour()
        {
            if (!string.IsNullOrWhiteSpace(NewTourName))
            {
                var newTour = new Tour { Name = NewTourName };
                Tours.Add(newTour);
                _dataService.AddTour(newTour); 
                NewTourName = string.Empty;
            }
        }

        private void DeleteTour()
        {
            if (SelectedTour != null)
            {
                _dataService.DeleteTour(SelectedTour); 
                Tours.Remove(SelectedTour);
                SelectedTour = null;
            }
        }
    }
}

