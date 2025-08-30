namespace SWEN2_TourPlannerGroupProject.DTOs
{
    public class TourLogDto
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
        public int? TourId { get; set; }
    }
}
