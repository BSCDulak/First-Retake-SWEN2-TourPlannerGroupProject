using SWEN2_TourPlannerGroupProject.Models;
using SWEN2_TourPlannerGroupProject.DTOs;

namespace SWEN2_TourPlannerGroupProject.Mappers
{
    public static class TourMapper
    {
        public static TourDto ToDto(Tour tour) => new TourDto
        {
            TourId = tour.TourId,
            BackupId = tour.BackupId,
            Name = tour.Name,
            Description = tour.Description,
            StartLocation = tour.StartLocation,
            EndLocation = tour.EndLocation,
            TransportType = tour.TransportType,
            Distance = tour.Distance,
            EstimatedTime = tour.EstimatedTime,
            RouteInformation = tour.RouteInformation,
            RouteImagePath = tour.RouteImagePath,
            Popularity = tour.Popularity,
            ChildFriendliness = tour.ChildFriendliness,
            TourLogs = tour.TourLogs.Select(ToDto).ToList()
        };

        public static TourLogDto ToDto(TourLog log) => new TourLogDto
        {
            TourLogId = log.TourLogId,
            Date = log.Date,
            TotalTime = log.TotalTime,
            Report = log.Report,
            Distance = log.Distance,
            Rating = log.Rating,
            AverageSpeed = log.AverageSpeed,
            DateTime = log.DateTime,
            Comment = log.Comment,
            Difficulty = log.Difficulty,
            TotalDistance = log.TotalDistance,
            TimeSpan = log.TimeSpan,
            Name = log.Name,
            TourId = log.TourId
        };

        public static Tour FromDto(TourDto dto)
        {
            var tour = new Tour
            {
                TourId = dto.TourId,
                BackupId = dto.BackupId,
                Name = dto.Name,
                Description = dto.Description,
                StartLocation = dto.StartLocation,
                EndLocation = dto.EndLocation,
                TransportType = dto.TransportType,
                Distance = dto.Distance,
                EstimatedTime = dto.EstimatedTime,
                RouteInformation = dto.RouteInformation,
                RouteImagePath = dto.RouteImagePath,
                Popularity = dto.Popularity,
                ChildFriendliness = dto.ChildFriendliness
            };

            foreach (var logDto in dto.TourLogs)
            {
                var log = FromDto(logDto);
                log.Tour = tour; // restore relation
                tour.TourLogs.Add(log);
            }

            return tour;
        }

        public static TourLog FromDto(TourLogDto dto) => new TourLog
        {
            TourLogId = dto.TourLogId,
            Date = dto.Date,
            TotalTime = dto.TotalTime,
            Report = dto.Report,
            Distance = dto.Distance,
            Rating = dto.Rating,
            AverageSpeed = dto.AverageSpeed,
            DateTime = dto.DateTime,
            Comment = dto.Comment,
            Difficulty = dto.Difficulty,
            TotalDistance = dto.TotalDistance,
            TimeSpan = dto.TimeSpan,
            Name = dto.Name,
            TourId = dto.TourId
        };
    }
}
