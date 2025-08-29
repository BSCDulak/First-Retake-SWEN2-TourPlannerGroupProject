using SWEN2_TourPlannerGroupProject.Data;
using SWEN2_TourPlannerGroupProject.DTOs;
using SWEN2_TourPlannerGroupProject.Mappers;
using SWEN2_TourPlannerGroupProject.Models;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.IO;

namespace SWEN2_TourPlannerGroupProject.Helpers
{
    public static class ImportHelper
    {
        public static async Task ImportToursAsync(
            ObservableCollection<Tour> targetCollection,
            string filePath,
            ITourRepository? repository = null,
            ILoggerWrapper? log = null)
        {
            if (!File.Exists(filePath))
            {
                log?.Warn($"File not found: {filePath}");
                return;
            }

            string json = File.ReadAllText(filePath);
            var tourDtos = JsonSerializer.Deserialize<List<TourDto>>(json);

            if (tourDtos == null || tourDtos.Count == 0)
            {
                log?.Warn($"No tours found in file: {filePath}");
                return;
            }

            foreach (var dto in tourDtos)
            {
                // Map to domain model
                var tour = TourMapper.FromDto(dto);

                // Reset IDs to avoid duplicates
                tour.TourId = null;
                foreach (var logItem in tour.TourLogs)
                {
                    logItem.TourLogId = null;
                    logItem.TourId = null; // EF will assign correct FK
                }

                // Add to collection
                targetCollection.Add(tour);

                // Save to repository
                if (repository != null)
                    await repository.AddTourAsync(tour);
            }

            log?.Info($"Imported {tourDtos.Count} tours from {filePath}");
        }
    }
}
