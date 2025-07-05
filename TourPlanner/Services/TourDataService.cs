using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SWEN2_TourPlannerGroupProject.Models;
using SWEN2_TourPlannerGroupProject.Data;

namespace SWEN2_TourPlannerGroupProject.Services
{
    public class TourDataService
    {
        private readonly AppDbContextFactory _contextFactory;

        public TourDataService()
        {
            _contextFactory = new AppDbContextFactory();
        }

        // Get all tours
        public List<Tour> GetTours()
        {
            using var context = _contextFactory.CreateDbContext(null);
            return context.Tours.Include(t => t.TourLogs).ToList();
        }

        // Add a new tour
        public void AddTour(Tour tour)
        {
            using var context = _contextFactory.CreateDbContext(null);
            context.Tours.Add(tour);
            context.SaveChanges();
        }

        // Update an existing tour
        public void UpdateTour(Tour tour)
        {
            using var context = _contextFactory.CreateDbContext(null);
            context.Tours.Update(tour);
            context.SaveChanges();
        }

        // Delete a tour
        public void DeleteTour(Tour tour)
        {
            using var context = _contextFactory.CreateDbContext(null);
            context.Tours.Remove(tour);
            context.SaveChanges();
        }

        // Get all tour logs for a tour
        public List<TourLog> GetTourLogs(int tourId)
        {
            using var context = _contextFactory.CreateDbContext(null);
            return context.TourLogs.Where(l => l.TourId == tourId).ToList();
        }

        // Add a new tour log
        public void AddTourLog(TourLog log)
        {
            using var context = _contextFactory.CreateDbContext(null);
            context.TourLogs.Add(log);
            context.SaveChanges();
        }

        // Update an existing tour log
        public void UpdateTourLog(TourLog log)
        {
            using var context = _contextFactory.CreateDbContext(null);
            context.TourLogs.Update(log);
            context.SaveChanges();
        }

        // Delete a tour log
        public void DeleteTourLog(TourLog log)
        {
            using var context = _contextFactory.CreateDbContext(null);
            context.TourLogs.Remove(log);
            context.SaveChanges();
        }
    }
}



