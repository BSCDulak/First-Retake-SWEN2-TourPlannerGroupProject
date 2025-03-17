using System.Collections.ObjectModel;
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
        
        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }

        public ToursListViewModel(ObservableCollection<Tour> tours)
        {
            Tours = tours;
            AddCommand = new RelayCommand(_ => AddTour());
            DeleteCommand = new RelayCommand(_ => DeleteTour(), _ => SelectedTour != null);
        }
        private void AddTour()
        {
            var newTour = new Tour { Name = "newTour"};
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
    }
}
