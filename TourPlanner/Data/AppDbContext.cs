using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SWEN2_TourPlannerGroupProject.Models;

namespace SWEN2_TourPlannerGroupProject.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Tour> Tours { get; set; }
        public DbSet<TourLog> TourLogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data Source=C:\Mac\Home\Documents\SWEN2-TourPlannerGroupProject\TourPlanner\tourplanner.db");

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tour>()
                .HasMany(t => t.TourLogs)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
