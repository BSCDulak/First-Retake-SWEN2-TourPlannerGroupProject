using SWEN2_TourPlannerGroupProject.Models;
using SWEN2_TourPlannerGroupProject.MVVM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SWEN2_TourPlannerGroupProject.ViewModels
{
    internal class MapViewModel : ViewModelBase
    {
        private static readonly ILoggerWrapper log = LoggerFactory.GetLogger();
        private Tour? _selectedTour;
        private readonly HttpClient _httpClient;
        private string API_KEY = App.ApiKey;

        public Tour? SelectedTour
        {
            get => _selectedTour;
            set
            {
                // Unsubscribe from previous tour's property changes
                if (_selectedTour != null)
                {
                    _selectedTour.PropertyChanged -= OnTourPropertyChanged;
                }

                SetField(ref _selectedTour, value);
                OnPropertyChanged(nameof(SelectedTour));
                
                // Subscribe to new tour's property changes
                if (_selectedTour != null)
                {
                    _selectedTour.PropertyChanged += OnTourPropertyChanged;
                }
                
                UpdateMap();
            }
        }

        public MapViewModel()
        {
            _httpClient = new HttpClient();
        }

        public void SetSelectedTour(Tour? tour)
        {
            SelectedTour = tour;
        }

        private void OnTourPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // Only update map when start or end location changes
            if (e.PropertyName == nameof(Tour.StartLocation) || e.PropertyName == nameof(Tour.EndLocation))
            {
                log.Info($"MAP: Tour property changed: {e.PropertyName}, updating map...");
                UpdateMap();
            }
        }

        private async void UpdateMap()
        {
            if (SelectedTour == null)
            {
                // Show default Vienna map
                ShowDefaultMap();
                return;
            }

            var startLocation = SelectedTour.StartLocation;
            var endLocation = SelectedTour.EndLocation;

            // Check if both locations are provided
            if (string.IsNullOrWhiteSpace(startLocation) || string.IsNullOrWhiteSpace(endLocation))
            {
                ShowDefaultMap();
                return;
            }

            try
            {
                // Try to get coordinates for both locations
                var startCoords = await GetCoordinatesAsync(startLocation);
                var endCoords = await GetCoordinatesAsync(endLocation);

                if (startCoords.HasValue && endCoords.HasValue)
                {
                    // Both locations found, show route and calculate transport info
                    await ShowRouteAsync(startCoords.Value, endCoords.Value, startLocation, endLocation);
                }
                else
                {
                    // One or both locations not found, show default map
                    ShowDefaultMap();
                }
            }
            catch (Exception ex)
            {
                log.Info($"MAP: Error updating map: {ex.Message}");
                ShowDefaultMap();
            }
        }

        private async Task<(double lat, double lng)?> GetCoordinatesAsync(string address)
        {
            try
            {
                var url = $"https://api.openrouteservice.org/geocode/search?api_key={API_KEY}&text={Uri.EscapeDataString(address)}&size=1";
                log.Info($"MAP: Geocoding URL: {url}");
                var response = await _httpClient.GetStringAsync(url);
                log.Info($"MAP: Geocoding response: {response}");
                var jsonDoc = JsonDocument.Parse(response);

                var features = jsonDoc.RootElement.GetProperty("features");
                if (features.GetArrayLength() > 0)
                {
                    var coordinates = features[0].GetProperty("geometry").GetProperty("coordinates");
                    var lng = coordinates[0].GetDouble();
                    var lat = coordinates[1].GetDouble();
                    log.Info($"MAP: Found coordinates for {address}: {lat.ToString(CultureInfo.InvariantCulture)}, {lng.ToString(CultureInfo.InvariantCulture)}");
                    return (lat, lng);
                }
                else
                {
                    log.Info($"MAP: No coordinates found for {address}");
                }
            }
            catch (Exception ex)
            {
                log.Info($"MAP: Error getting coordinates for {address}: {ex.Message}");
            }
            return null;
        }

        private async Task ShowRouteAsync((double lat, double lng) start, (double lat, double lng) end, string startAddress, string endAddress)
        {
            try
            {
                // First try walking route
                var walkingRoute = await GetRouteAsync(start, end, "foot-walking");
                if (walkingRoute.HasValue && walkingRoute.Value.duration <= 1800) // 30 minutes = 1800 seconds
                {
                    // Walking is feasible (under 30 minutes)
                    var routeJs = GenerateRouteJavaScript(start, end, walkingRoute.Value.coordinates, startAddress, endAddress, "Walking");
                    LoadMapWithRoute(routeJs);
                    
                    // Update tour with walking information
                    UpdateTourWithRouteInfo(walkingRoute.Value, "Walking");
                }
                else
                {
                    // Try public transport
                    var publicTransportRoute = await GetRouteAsync(start, end, "driving-car");
                    if (publicTransportRoute.HasValue)
                    {
                        var routeJs = GenerateRouteJavaScript(start, end, publicTransportRoute.Value.coordinates, startAddress, endAddress, "Public Transport");
                        LoadMapWithRoute(routeJs);
                        
                        // Update tour with public transport information
                        UpdateTourWithRouteInfo(publicTransportRoute.Value, "Public Transport");
                    }
                    else
                    {
                        // Fallback to car
                        var carRoute = await GetRouteAsync(start, end, "driving-car");
                        if (carRoute.HasValue)
                        {
                            var routeJs = GenerateRouteJavaScript(start, end, carRoute.Value.coordinates, startAddress, endAddress, "Car");
                            LoadMapWithRoute(routeJs);
                            
                            // Update tour with car information
                            UpdateTourWithRouteInfo(carRoute.Value, "Car");
                        }
                        else
                        {
                            ShowDefaultMap();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Info($"MAP: Error getting route: {ex.Message}");
                ShowDefaultMap();
            }
        }

        private async Task<((double lat, double lng)[] coordinates, double distance, double duration)?> GetRouteAsync((double lat, double lng) start, (double lat, double lng) end, string profile)
        {
            try
            {
                var requestBody = new
                {
                    coordinates = new[]
                    {
                        new[] { start.lng, start.lat },
                        new[] { end.lng, end.lat }
                    },
                    format = "geojson",
                    profile = profile
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"https://api.openrouteservice.org/v2/directions/{profile}/geojson?api_key={API_KEY}", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var routeData = JsonDocument.Parse(responseContent);
                    var features = routeData.RootElement.GetProperty("features");
                    if (features.GetArrayLength() > 0)
                    {
                        var feature = features[0];
                        var coordinates = feature.GetProperty("geometry").GetProperty("coordinates");
                        var properties = feature.GetProperty("properties");
                        var summary = properties.GetProperty("summary");
                        
                        var distance = summary.GetProperty("distance").GetDouble(); // in meters
                        var duration = summary.GetProperty("duration").GetDouble(); // in seconds
                        
                        var coordArray = new List<(double lat, double lng)>();
                        foreach (var coord in coordinates.EnumerateArray())
                        {
                            var lng = coord[0].GetDouble();
                            var lat = coord[1].GetDouble();
                            coordArray.Add((lat, lng));
                        }
                        
                        return (coordArray.ToArray(), distance, duration);
                    }
                }
                else
                {
                    log.Info($"MAP: Route API error for {profile}: {responseContent}");
                }
            }
            catch (Exception ex)
            {
                log.Info($"MAP: Error getting {profile} route: {ex.Message}");
            }
            return null;
        }
        // everywhere we use lat/lng we need .ToString(CultureInfo.InvariantCulture) to avoid different behaviour in different locale, in particular issues with the API not getting correct values.
        private void UpdateTourWithRouteInfo(((double lat, double lng)[] coordinates, double distance, double duration) routeInfo, string transportType)
        {
            if (SelectedTour != null)
            {
                // Update transport type
                SelectedTour.TransportType = transportType;
                
                // Update distance (convert meters to km)
                var distanceKm = routeInfo.distance / 1000.0;
                SelectedTour.Distance = distanceKm.ToString("F1", CultureInfo.InvariantCulture) + "km";
                
                // Update estimated time (convert seconds to hours and minutes)
                var totalMinutes = routeInfo.duration / 60.0;
                var hours = (int)(totalMinutes / 60);
                var minutes = (int)(totalMinutes % 60);
                
                if (hours > 0)
                {
                    SelectedTour.EstimatedTime = $"{hours}h {minutes}min";
                }
                else
                {
                    SelectedTour.EstimatedTime = $"{minutes} min";
                }
                
                // Notify that the tour has been updated
                OnPropertyChanged(nameof(SelectedTour));
            }
        }
        // everywhere we use lat/lng we need .ToString(CultureInfo.InvariantCulture) to avoid different behaviour in different locale, in particular issues with the API not getting correct values.
        private string GenerateRouteJavaScript((double lat, double lng) start, (double lat, double lng) end, (double lat, double lng)[] coordinates, string startAddress, string endAddress, string transportType)
        {
            var routeCoords = new StringBuilder();
            foreach (var coord in coordinates)
            {
                routeCoords.Append($"[{coord.lat.ToString(CultureInfo.InvariantCulture)}, {coord.lng.ToString(CultureInfo.InvariantCulture)}],");
            }

            var routeColor = transportType switch
            {
                "Walking" => "green",
                "Public Transport" => "blue",
                "Car" => "red",
                _ => "blue"
            };

            return $@"
                var map = L.map('map').setView([{((start.lat + end.lat) / 2).ToString(CultureInfo.InvariantCulture)}, {((start.lng + end.lng) / 2).ToString(CultureInfo.InvariantCulture)}], 12);
                L.tileLayer('https://{{s}}.tile.openstreetmap.org/{{z}}/{{x}}/{{y}}.png', {{
                    maxZoom: 19,
                    attribution: '© OpenStreetMap contributors'
                }}).addTo(map);
                
                var routeCoords = [{routeCoords.ToString().TrimEnd(',')}];
                var route = L.polyline(routeCoords, {{color: '{routeColor}', weight: 5}}).addTo(map);
                
                var startMarker = L.marker([{start.lat.ToString(CultureInfo.InvariantCulture)}, {start.lng.ToString(CultureInfo.InvariantCulture)}]).addTo(map);
                startMarker.bindPopup('<b>Start</b><br>{startAddress}<br><b>Transport:</b> {transportType}');
                
                var endMarker = L.marker([{end.lat.ToString(CultureInfo.InvariantCulture)}, {end.lng.ToString(CultureInfo.InvariantCulture)}]).addTo(map);
                endMarker.bindPopup('<b>End</b><br>{endAddress}<br><b>Transport:</b> {transportType}');
                
                map.fitBounds(route.getBounds());
            ";
        }

        private void LoadMapWithRoute(string routeJavaScript)
        {
            var html = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <title>Route Map</title>
                    <meta charset='utf-8' />
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <link rel='stylesheet' href='https://unpkg.com/leaflet@1.9.4/dist/leaflet.css' 
                          integrity='sha256-p4NxAoJBhIIN+hmNHrzRCf9tD/miZyoHS5obTRR9BMY=' 
                          crossorigin='' />
                    <script src='https://unpkg.com/leaflet@1.9.4/dist/leaflet.js' 
                            integrity='sha256-20nQCchB9co0qIjJZRGuk2/Z9VM+kNiyxNV1lvTlZBo=' 
                            crossorigin=''></script>
                    <style>
                        html, body {{
                            height: 100%;
                            margin: 0;
                            padding: 0;
                            font-family: Arial, sans-serif;
                        }}
                        #map {{
                            height: 100%;
                            width: 100%;
                            margin: 0;
                            padding: 0;
                        }}
                        .leaflet-container {{
                            background: #f8f9fa;
                        }}
                    </style>
                </head>
                <body>
                    <div id='map'></div>
                    <script>
                        {routeJavaScript}
                    </script>
                </body>
                </html>";

            // This will be called from the MapUserControl
            OnMapUpdateRequested?.Invoke(html);
        }

        private void ShowDefaultMap()
        {
            var html = @"
                <!DOCTYPE html>
                <html>
                <head>
                    <title>Leaflet Map</title>
                    <meta charset='utf-8' />
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <link rel='stylesheet' href='https://unpkg.com/leaflet@1.9.4/dist/leaflet.css' 
                          integrity='sha256-p4NxAoJBhIIN+hmNHrzRCf9tD/miZyoHS5obTRR9BMY=' 
                          crossorigin='' />
                    <script src='https://unpkg.com/leaflet@1.9.4/dist/leaflet.js' 
                            integrity='sha256-20nQCchB9co0qIjJZRGuk2/Z9VM+kNiyxNV1lvTlZBo=' 
                            crossorigin=''></script>
                    <style>
                        html, body {
                            height: 100%;
                            margin: 0;
                            padding: 0;
                            font-family: Arial, sans-serif;
                        }
                        #map {
                            height: 100%;
                            width: 100%;
                            margin: 0;
                            padding: 0;
                        }
                        .leaflet-container {
                            background: #f8f9fa;
                        }
                    </style>
                </head>
                <body>
                    <div id='map'></div>
                    <script>
                        var map = L.map('map').setView([48.2082, 16.3738], 13);
                        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
                            maxZoom: 19,
                            attribution: '© OpenStreetMap contributors'
                        }).addTo(map);
                        
                        var marker = L.marker([48.2082, 16.3738]).addTo(map);
                        marker.bindPopup('<b>Vienna</b><br>Welcome to the Tour Planner!').openPopup();
                    </script>
                </body>
                </html>";

            OnMapUpdateRequested?.Invoke(html);
        }

        // Event to notify MapUserControl to update the map
        public event Action<string>? OnMapUpdateRequested;
    }
}
