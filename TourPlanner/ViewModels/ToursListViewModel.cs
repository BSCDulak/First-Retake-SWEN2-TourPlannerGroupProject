using System.Collections.ObjectModel;
using System.Formats.Tar;
using System.Windows;
using System.Windows.Input;
using SWEN2_TourPlannerGroupProject.Models;
using SWEN2_TourPlannerGroupProject.MVVM;

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
                _newTourName = value;
                OnPropertyChanged(nameof(NewTourName));
            }
        }

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }

        public ToursListViewModel(ObservableCollection<Tour> tours)
        {
            Tours = tours;
            AddCommand = new RelayCommand(_ => AddTour(), _ => !string.IsNullOrWhiteSpace(NewTourName));
            DeleteCommand = new RelayCommand(_ => DeleteTour(), _ => SelectedTour != null);
        }

        private void AddTour()
        {
            throw new NotImplementedException();
        }

        private void AddTour(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(NewTourName))
            {
                var newTour = new Tour { Name = NewTourName };
                Tours.Add(newTour);
                NewTourName = string.Empty;

                
            }
        }

        private void DeleteTour()
        {
            if (SelectedTour != null)
            {
                Tours.Remove(SelectedTour);
                SelectedTour = null;
            }
        }
    }
}

