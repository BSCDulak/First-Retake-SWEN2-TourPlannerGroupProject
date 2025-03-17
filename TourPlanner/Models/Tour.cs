using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN2_TourPlannerGroupProject.Models
{
    // This class is a model for a Tour, it contains all the information that is needed for a Tour and is used
    // by toursList(ViewModel) and TourDetailsWrapPanel(ViewModel) to display the information in the GUI.
    internal class Tour
    {
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

        /* todo This is a list of TourLogs that are associated with this Tour, needs a model, viewmodel and usercontrol.
        public List<TourLog> TourLogs { get; set; } = new List<TourLog>();
        */

    }
}
