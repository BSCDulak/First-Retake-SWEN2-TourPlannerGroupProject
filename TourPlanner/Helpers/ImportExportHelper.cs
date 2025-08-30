using log4net;
using SWEN2_TourPlannerGroupProject.Data;
using SWEN2_TourPlannerGroupProject.DTOs;
using SWEN2_TourPlannerGroupProject.Mappers;
using SWEN2_TourPlannerGroupProject.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;

namespace SWEN2_TourPlannerGroupProject.Helpers
{
    public static class ImportExportHelper
    {
        public static async Task ImportToursAsync(
            ObservableCollection<Tour> targetCollection,
            string filePath,
            ITourRepository? repository = null,
            ILoggerWrapper? log = null)
        {
            log.Info("ImportExportHelper: Starting import of tours.");
            if (!File.Exists(filePath))
            {
                log?.Warn($"ImportExportHelper: File not found: {filePath}");
                return;
            }

            string json = File.ReadAllText(filePath);
            var tourDtos = JsonSerializer.Deserialize<List<TourDto>>(json);

            if (tourDtos == null || tourDtos.Count == 0)
            {
                log?.Warn($"ImportExportHelper: No tours found in file: {filePath}");
                return;
            }
            int successImportedCount = 0;
            foreach (var dto in tourDtos)
            {
                log.Info($"ImportExportHelper: Processing imported tour DTO: {dto.Name} (DTO BackupID {dto.BackupId})");
                // Map to domain model
                var tour = TourMapper.FromDto(dto);

                // Reset IDs to avoid duplicates
                tour.TourId = null;
                bool existsInMemory = targetCollection.Any(t => t.BackupId == tour.BackupId);
                if (existsInMemory)
                {
                    log.Warn($"Skipping duplicate tour: {tour.Name} (ID: {tour.TourId} / BackupID {tour.BackupId}. existsInMemory:{existsInMemory})");
                    continue;
                }
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
                successImportedCount++;
            }

            log?.Info($"Imported {successImportedCount} tours from {filePath}");
        }
        public static void ExportTours(IEnumerable<Tour> tours, string filePath, ILoggerWrapper? log = null)
        {
            if (tours == null)
            {
                log?.Warn("Tried to export null tour list.");
                return;
            }
            var dtoList = tours.Select(TourMapper.ToDto).ToList();
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(dtoList, options);
            try
            {
                File.WriteAllText(filePath, json);
                log?.Info($"Exported {dtoList.Count} tours to {filePath}");
            }
            catch (Exception ex)
            {
                log?.Error($"Failed to export tours to {filePath}: {ex.Message}");
            }
            log.Info($"Exported {tours.Count()} tours to {filePath}");
        }
    }

}
