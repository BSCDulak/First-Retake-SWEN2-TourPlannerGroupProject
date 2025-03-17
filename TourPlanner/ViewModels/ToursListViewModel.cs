using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SWEN2_TourPlannerGroupProject.Models;
using SWEN2_TourPlannerGroupProject.MVVM;

namespace SWEN2_TourPlannerGroupProject.ViewModels
{
    internal class ToursListViewModel : ViewModelBase
    {
        public ObservableCollection<Tours> Tours { get; }
        private Tours? _selectedTour;
        public Tours? SelectedTour
        {
            get => _selectedTour;
            set
            {
                SetField(ref _selectedTour, value);
                OnPropertyChanged(nameof(SelectedTour));
                CommandManager.InvalidateRequerySuggested();
            }
        }
        private string? _newTourName;
        public string? NewTourName
        {
            get => _newTourName;
            set => SetField(ref _newTourName, value);
        }
        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }

        public ToursListViewModel(ObservableCollection<Tours> tours)
        {
            Tours = tours;
            AddCommand = new RelayCommand(_ => AddTour());
            DeleteCommand = new RelayCommand(_ => DeleteTour(), _ => SelectedTour != null);
        }
        private void AddTour()
        {
            Tours.Add(new Tours {Name = NewTourName});
            NewTourName = string.Empty;
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
