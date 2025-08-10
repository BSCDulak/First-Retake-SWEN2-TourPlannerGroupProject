using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using System.Windows.Input;
using SWEN2_TourPlannerGroupProject.Models;
using SWEN2_TourPlannerGroupProject.MVVM;

namespace SWEN2_TourPlannerGroupProject.ViewModels
{
    internal class MapViewModel : ViewModelBase
    {
        private Tour? _selectedTour;
        private readonly HttpClient _httpClient;
        private const string API_KEY = "eyJvcmciOiI1YjNjZTM1OTc4NTExMTAwMDFjZjYyNDgiLCJpZCI6ImQ4NjYyZmU3YmRhNzRjZWFhNzU5NGM4ZjRkYzE4OThhIiwiaCI6Im11cm11cjY0In0=";

        public Tour? SelectedTour
        {
            get => _selectedTour;
            set
            {
                SetField(ref _selectedTour, value);
                OnPropertyChanged(nameof(SelectedTour));
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
                    // Both locations found, show route
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
                Console.WriteLine($"Error updating map: {ex.Message}");
                ShowDefaultMap();
            }
        }

        private async Task<(double lat, double lng)?> GetCoordinatesAsync(string address)
        {
            try
            {
                var url = $"https://api.openrouteservice.org/geocode/search?api_key={API_KEY}&text={Uri.EscapeDataString(address)}&size=1";
                Console.WriteLine($"Geocoding URL: {url}");
                var response = await _httpClient.GetStringAsync(url);
                Console.WriteLine($"Geocoding response: {response}");
                var jsonDoc = JsonDocument.Parse(response);
                
                var features = jsonDoc.RootElement.GetProperty("features");
                if (features.GetArrayLength() > 0)
                {
                    var coordinates = features[0].GetProperty("geometry").GetProperty("coordinates");
                    var lng = coordinates[0].GetDouble();
                    var lat = coordinates[1].GetDouble();
                    Console.WriteLine($"Found coordinates for {address}: {lat}, {lng}");
                    return (lat, lng);
                }
                else
                {
                    Console.WriteLine($"No coordinates found for {address}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting coordinates for {address}: {ex.Message}");
            }
            return null;
        }

        private async Task ShowRouteAsync((double lat, double lng) start, (double lat, double lng) end, string startAddress, string endAddress)
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
                    profile = "driving-car"
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"https://api.openrouteservice.org/v2/directions/driving-car/geojson?api_key={API_KEY}", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var routeData = JsonDocument.Parse(responseContent);
                    var coordinates = routeData.RootElement.GetProperty("features")[0].GetProperty("geometry").GetProperty("coordinates");
                    
                    // Generate JavaScript for the route
                    var routeJs = GenerateRouteJavaScript(start, end, coordinates, startAddress, endAddress);
                    LoadMapWithRoute(routeJs);
                }
                else
                {
                    Console.WriteLine($"Route API error: {responseContent}");
                    ShowDefaultMap();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting route: {ex.Message}");
                ShowDefaultMap();
            }
        }

        private string GenerateRouteJavaScript((double lat, double lng) start, (double lat, double lng) end, JsonElement coordinates, string startAddress, string endAddress)
        {
            var routeCoords = new StringBuilder();
            foreach (var coord in coordinates.EnumerateArray())
            {
                var lng = coord[0].GetDouble();
                var lat = coord[1].GetDouble();
                routeCoords.Append($"[{lat}, {lng}],");
            }

            return $@"
                var map = L.map('map').setView([{(start.lat + end.lat) / 2}, {(start.lng + end.lng) / 2}], 12);
                L.tileLayer('https://{{s}}.tile.openstreetmap.org/{{z}}/{{x}}/{{y}}.png', {{
                    maxZoom: 19,
                    attribution: '© OpenStreetMap contributors'
                }}).addTo(map);
                
                var routeCoords = [{routeCoords.ToString().TrimEnd(',')}];
                var route = L.polyline(routeCoords, {{color: 'blue', weight: 5}}).addTo(map);
                
                var startMarker = L.marker([{start.lat}, {start.lng}]).addTo(map);
                startMarker.bindPopup('<b>Start</b><br>{startAddress}');
                
                var endMarker = L.marker([{end.lat}, {end.lng}]).addTo(map);
                endMarker.bindPopup('<b>End</b><br>{endAddress}');
                
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
