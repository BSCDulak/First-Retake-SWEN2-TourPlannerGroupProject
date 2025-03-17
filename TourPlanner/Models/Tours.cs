using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN2_TourPlannerGroupProject.Models
{
    internal class Tours
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
        public int TourId { get; set; }

    }
}
