using System;
using System.ComponentModel;
using System.Windows.Input;
using SWEN2_TourPlannerGroupProject.Models;
using SWEN2_TourPlannerGroupProject.MVVM;
using SWEN2_TourPlannerGroupProject.Services;
using System.IO;

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

        public ICommand CalculateRouteCommand => new RelayCommand(async _ => await CalculateRoute());


        private async Task CalculateRoute()
        {
            if (SelectedTour == null) return;

            try
            {
                var apiKey = "eyJvcmciOiI1YjNjZTM1OTc4NTExMTAwMDFjZjYyNDgiLCJpZCI6ImE2Njg2Zjg3MDUxZjRhY2M4MDk0ZWMyOGZjN2U1MDBmIiwiaCI6Im11cm11cjY0In0=";
                var client = new OpenRouteServiceClient(apiKey);

                var from = await client.GeocodeAsync(SelectedTour.StartLocation);
                var to = await client.GeocodeAsync(SelectedTour.EndLocation);

                SelectedTour.FromLatitude = from.lat;
                SelectedTour.FromLongitude = from.lon;
                SelectedTour.ToLatitude = to.lat;
                SelectedTour.ToLongitude = to.lon;

                var routeJson = await client.GetRouteAsync(from.lat, from.lon, to.lat, to.lon);
                var summary = routeJson["features"]?[0]?["properties"]?["summary"];

                if (summary != null)
                {
                    double distanceKm = summary.Value<double>("distance") / 1000.0;
                    double durationSec = summary.Value<double>("duration");

                    SelectedTour.Distance = $"{distanceKm:F2} km";
                    SelectedTour.EstimatedTime = $"{TimeSpan.FromSeconds(durationSec):hh\\:mm}";
                }

                // Guardar GeoJSON
                var outputPath = System.IO.Path.Combine(AppContext.BaseDirectory, "MapHtml", "route.geojson");
                System.IO.File.WriteAllText(outputPath, routeJson.ToString());

                var service = new TourDataService();
                service.UpdateTour(SelectedTour);

                OnPropertyChanged(nameof(SelectedTour));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error calculating route: {ex.Message}");
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



