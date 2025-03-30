using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SWEN2_TourPlannerGroupProject.Data;
using SWEN2_TourPlannerGroupProject.Models;

namespace SWEN2_TourPlannerGroupProject.Services
{
    public class TourDataService
    {
        public ObservableCollection<Tour> GetAllTours()
        {
            using var db = new AppDbContext();
            var tours = db.Tours.Include(t => t.TourLogs).ToList();
            return new ObservableCollection<Tour>(tours);
        }

        public void AddTour(Tour tour)
        {
            using var db = new AppDbContext();
            db.Tours.Add(tour);
            db.SaveChanges();
        }

        public void DeleteTour(Tour tour)
        {
            using var db = new AppDbContext();
            var existing = db.Tours.Include(t => t.TourLogs).FirstOrDefault(t => t.Id == tour.Id);
            if (existing != null)
            {
                db.Tours.Remove(existing);
                db.SaveChanges();
            }
        }

        public void UpdateTour(Tour tour)
        {
            using var db = new AppDbContext();
            db.Tours.Update(tour);
            db.SaveChanges();
        }

        public void AddTourLog(int tourId, TourLog log)
        {
            using var db = new AppDbContext();
            var tour = db.Tours.Include(t => t.TourLogs).FirstOrDefault(t => t.Id == tourId);
            if (tour != null)
            {
                tour.TourLogs.Add(log);
                db.SaveChanges();
            }
        }

        public void DeleteTourLog(int tourId, TourLog log)
        {
            using var db = new AppDbContext();
            var tour = db.Tours.Include(t => t.TourLogs).FirstOrDefault(t => t.Id == tourId);
            if (tour != null)
            {
                var existingLog = tour.TourLogs.FirstOrDefault(l => l.Id == log.Id);
                if (existingLog != null)
                {
                    tour.TourLogs.Remove(existingLog);
                    db.SaveChanges();
                }
            }
        }
    }
}


