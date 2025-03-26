using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using SWEN2_TourPlannerGroupProject.Models;
using SWEN2_TourPlannerGroupProject.MVVM;

namespace SWEN2_TourPlannerGroupProject.ViewModels
{
    // This class is the viewmodel for the ToursList, it contains the list of tours and the commands for adding and deleting tours.
    // this commands are triggered by the buttons in the GUI. the remove button has a condition of SelectedTour != null, which means
    // that the button is only enabled when a tour is selected in the list.
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

        private string? _newAddText;
        public string NewAddText
        {
            get => _newAddText;
            set
            {
                _newAddText = value;
                OnPropertyChanged(nameof(NewAddText));  // Notify the UI that the value has changed
            }
        }




        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ToursListViewModel()
        {
            // Initialize properties if necessary
        }

        public ToursListViewModel(ObservableCollection<Tour> tours)
        {
            Tours = tours;
            AddCommand = new RelayCommand(param =>
            {
                // Here you use 'param' which is the value passed from the UI
                string tourName = param as string;

                // Ensure tourName is not null or empty before adding the tour
                if (string.IsNullOrWhiteSpace(tourName))
                {
                    // You can add some validation or feedback for the user if necessary
                    Debug.WriteLine("Tour name is invalid");
                    return;
                }

                // Add the tour with the given tourName
                AddTour(tourName);
            }, _ => _newAddText != null);
            DeleteCommand = new RelayCommand(_ => DeleteTour(), _ => SelectedTour != null);
        }
        public void AddTour(string tourName)
        {
            Debug.WriteLine($"Received tourName: '{tourName}'");  // Add this line to check the value
            if (!string.IsNullOrWhiteSpace(tourName))
            {
                Debug.WriteLine($"Adding tour: {tourName}"); // Log to debug
                Tours.Add(new Tour { Name = tourName });
            }
            else
            {
                Debug.WriteLine("Tour name is invalid");
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
