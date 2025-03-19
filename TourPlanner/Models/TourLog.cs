using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN2_TourPlannerGroupProject.Models
{
    public class TourLog
    {
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
        

    }
}
