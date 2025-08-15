using Microsoft.EntityFrameworkCore;
using SWEN2_TourPlannerGroupProject.Models;

namespace SWEN2_TourPlannerGroupProject.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Tour> Tours { get; set; }
        public DbSet<TourLog> TourLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tour>(entity =>
            {
                entity.HasKey(t => t.TourId);
                entity.Property(t => t.TourId).ValueGeneratedOnAdd();
                entity.HasMany(l => l.TourLogs)
                    .WithOne()
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<TourLog>(entity =>
            {
                entity.HasKey(l => l.TourLogId);
                entity.Property(l => l.TourLogId).ValueGeneratedOnAdd();
            });
        }
    }
}
