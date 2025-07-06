using Microsoft.Web.WebView2.Wpf;
using System;
using System.IO;
using System.Windows.Controls;

namespace SWEN2_TourPlannerGroupProject.Views.Subviews
{
    public partial class OpenRouteServiceMap : UserControl
    {
        public OpenRouteServiceMap()
        {
            InitializeComponent();
            LoadMap();
        }

        private void LoadMap()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MapHtml", "map.html");
            MapView.Source = new Uri(path);
        }

    }
}


