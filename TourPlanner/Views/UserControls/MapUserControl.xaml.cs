using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using SWEN2_TourPlannerGroupProject.ViewModels;

namespace SWEN2_TourPlannerGroupProject.Views.UserControls
{
    /// <summary>
    /// Interaktionslogik für MapUserControl.xaml
    /// </summary>
    public partial class MapUserControl : UserControl
    {
        private MapViewModel? _mapViewModel;

        public MapUserControl()
        {
            InitializeComponent();
            SetupWebBrowser();
            LoadMap();
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            
            if (e.Property == DataContextProperty)
            {
                if (e.NewValue is MapViewModel mapViewModel)
                {
                    _mapViewModel = mapViewModel;
                    _mapViewModel.OnMapUpdateRequested += UpdateMap;
                }
                else if (e.OldValue is MapViewModel oldViewModel)
                {
                    oldViewModel.OnMapUpdateRequested -= UpdateMap;
                }
            }
        }

        private void UpdateMap(string html)
        {
            try
            {
                MapBrowser.NavigateToString(html);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating map: {ex.Message}");
            }
        }

        private void SetupWebBrowser()
        {
            // Enable modern web features for the WebBrowser control
            try
            {
                // Set feature control for WebBrowser
                SetWebBrowserFeatureControl();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error setting up WebBrowser: {ex.Message}");
            }
        }

        private void SetWebBrowserFeatureControl()
        {
            // Enable modern web features
            SetFeatureBrowserEmulation();
        }

        private void SetFeatureBrowserEmulation()
        {
            try
            {
                // Set the browser emulation mode to IE11
                Microsoft.Win32.Registry.SetValue(
                    @"HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION",
                    System.Diagnostics.Process.GetCurrentProcess().ProcessName + ".exe",
                    11001,
                    Microsoft.Win32.RegistryValueKind.DWord);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error setting browser emulation: {ex.Message}");
            }
        }

        private void LoadMap()
        {
            try
            {
                // Embed the HTML with Leaflet inside the WebBrowser
                string html = @"
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
                            try {
                                // Initialize the map
                                var map = L.map('map').setView([48.2082, 16.3738], 13); // Vienna coordinates
                                
                                // Add OpenStreetMap tiles
                                L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
                                    maxZoom: 19,
                                    attribution: '© OpenStreetMap contributors'
                                }).addTo(map);
                                
                                // Add a marker to show the map is working
                                var marker = L.marker([48.2082, 16.3738]).addTo(map);
                                marker.bindPopup('<b>Vienna</b><br>Welcome to the Tour Planner!').openPopup();
                                
                                console.log('Map loaded successfully');
                            } catch (error) {
                                console.error('Error loading map:', error);
                                document.getElementById('map').innerHTML = '<div style=""padding: 20px; text-align: center; color: #666;"">Map loading failed. Please check your internet connection.</div>';
                            }
                        </script>
                    </body>
                    </html>";

                MapBrowser.NavigateToString(html);
                
                // Add event handlers for debugging
                MapBrowser.LoadCompleted += (sender, e) =>
                {
                    Console.WriteLine("Map browser loaded successfully");
                };
                
                MapBrowser.Navigating += (sender, e) =>
                {
                    Console.WriteLine($"Map browser navigating to: {e.Uri}");
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in LoadMap: {ex.Message}");
                // Fallback: show a simple message in the WebBrowser
                string fallbackHtml = @"
                    <html>
                    <body style='margin: 0; padding: 20px; font-family: Arial, sans-serif; background-color: #f8f9fa; height: 100vh; display: flex; align-items: center; justify-content: center;'>
                        <div style='text-align: center; color: #666;'>
                            <div style='font-size: 48px; margin-bottom: 10px;'>🗺️</div>
                            <div style='font-size: 24px; font-weight: bold; margin-bottom: 5px;'>Map View</div>
                            <div style='font-size: 14px; margin-bottom: 5px;'>Interactive map will be displayed here</div>
                            <div style='font-size: 12px; color: #999;'>Please ensure you have an internet connection</div>
                        </div>
                    </body>
                    </html>";
                MapBrowser.NavigateToString(fallbackHtml);
            }
        }
    }
}
