using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN2_TourPlannerGroupProject.Models
{
    public class TourLog
    {
        public int? TourLogId { get; set; }
        public string? Date { get; set; }
        public string? TotalTime { get; set; }
        public string? Report { get; set; }
        public string? Distance { get; set; }
        public string? Rating { get; set; }
        public string? AverageSpeed { get; set; }
        public DateTime DateTime { get; set; }
        public string? Comment { get; set; }
        public string? Difficulty { get; set; }
        public double TotalDistance { get; set; }
        public TimeSpan TimeSpan { get; set; }
        public string? Name { get; set; }

        // Foreign key to Tour - needed for database relationship, if this is not needed in the application logic it can be removed later 
        // but it prolly is needed because OnModelCreating is creating that in the online database and I dunno how else we can pull this
        // info into the model otherwise, so let´s just keep it for now
        public int? TourId { get; set; }
        public Tour? Tour { get; set; }
    }
}
