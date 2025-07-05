using System;
using System.ComponentModel;
using System.Windows.Input;
using SWEN2_TourPlannerGroupProject.Models;
using SWEN2_TourPlannerGroupProject.MVVM;
using SWEN2_TourPlannerGroupProject.Services;

namespace SWEN2_TourPlannerGroupProject.ViewModels
{
    // ViewModel for the details panel in Tab1
    internal class TourDetailsWrapPanelViewModel : ViewModelBase
    {
        private readonly ToursListViewModel _toursListViewModel;

        public Tour? SelectedTour
        {
            get => _toursListViewModel?.SelectedTour;
            set
            {
                if (_toursListViewModel != null)
                {
                    _toursListViewModel.SelectedTour = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(MapUri));  // Notify when SelectedTour changes
                }
            }
        }

        public TourDetailsWrapPanelViewModel(ToursListViewModel toursListViewModel)
        {
            _toursListViewModel = toursListViewModel;
            _toursListViewModel.PropertyChanged += ToursListViewModelPropertyChanged;
        }

        // Design-time constructor
        public TourDetailsWrapPanelViewModel()
        {
            _toursListViewModel = null!;
        }

        private void ToursListViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_toursListViewModel.SelectedTour))
            {
                OnPropertyChanged(nameof(SelectedTour));
                OnPropertyChanged(nameof(MapUri)); // Notify MapUri when SelectedTour changes
            }
        }

        public ICommand SaveTourCommand => new RelayCommand(_ => SaveTour());

        private void SaveTour()
        {
            if (SelectedTour != null)
            {
                var service = new TourDataService();
                service.UpdateTour(SelectedTour);
            }
        }

        /// <summary>
        /// MapUri property to bind in WebView2.
        /// Generates a Google Maps query from StartLocation and EndLocation.
        /// </summary>
        public Uri MapUri
        {
            get
            {
                if (SelectedTour == null
                    || string.IsNullOrWhiteSpace(SelectedTour.StartLocation)
                    || string.IsNullOrWhiteSpace(SelectedTour.EndLocation))
                {
                    // Default map view if no locations are set
                    return new Uri("https://www.openstreetmap.org");
                }

                string query = $"{SelectedTour.StartLocation} to {SelectedTour.EndLocation}";
                string url = $"https://www.google.com/maps/search/?api=1&query={Uri.EscapeDataString(query)}";
                return new Uri(url);
            }
        }
    }
}



