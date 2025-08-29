namespace SWEN2_TourPlannerGroupProject.DTOs
{
    public class TourDto
    {
        public int? TourId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? StartLocation { get; set; }
        public string? EndLocation { get; set; }
        public string? TransportType { get; set; }
        public string? Distance { get; set; }
        public string? EstimatedTime { get; set; }
        public string? RouteInformation { get; set; }
        public string? RouteImagePath { get; set; }
        public string? Popularity { get; set; }
        public string? ChildFriendliness { get; set; }

        public List<TourLogDto> TourLogs { get; set; } = new();
    }
}
