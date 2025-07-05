using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN2_TourPlannerGroupProject.Models
{
    // This class is a model for a Tour, it contains all the information that is needed for a Tour and is used
    // by toursList(ViewModel) and TourDetailsWrapPanel(ViewModel) to display the information in the GUI.
    public class Tour
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? StartLocation { get; set; }
        public string? EndLocation { get; set; }
        public string? TransportType { get; set; }
        public string? Distance { get; set; }
        public string? EstimatedTime { get; set; }
        public string? RouteInformation { get; set; }
        public string? RouteImagePath { get; set; }
        public int? TourId { get; set; }
        public double FromLatitude { get; set; }
        public double FromLongitude { get; set; }
        public double ToLatitude { get; set; }
        public double ToLongitude { get; set; }



        /* todo This is a list of TourLogs that are associated with this Tour, needs a model, viewmodel and usercontrol.*/
        //public ObservableCollection<TourLog> TourLogs { get; set; } = new ObservableCollection<TourLog>();
        //public ObservableCollection<TourLog> TourLogs { get; set; } = new();
        public ObservableCollection<TourLog> TourLogs { get; set; } = new();

    }
}
