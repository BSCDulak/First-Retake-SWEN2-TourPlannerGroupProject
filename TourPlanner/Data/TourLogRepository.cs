using Microsoft.EntityFrameworkCore;
using SWEN2_TourPlannerGroupProject.Models;

namespace SWEN2_TourPlannerGroupProject.Data
{
    public class TourLogRepository : ITourLogRepository
    {
        private readonly AppDbContext _context;

        public TourLogRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TourLog>> GetTourLogsByTourIdAsync(int tourId)
        {
            return await _context.TourLogs
                .Where(tl => tl.TourId == tourId)
                .ToListAsync();
        }

        public async Task<TourLog> AddTourLogAsync(TourLog tourLog)
        {
            _context.TourLogs.Add(tourLog);
            await _context.SaveChangesAsync();
            return tourLog;
        }

        public async Task<TourLog> UpdateTourLogAsync(TourLog tourLog)
        {
            _context.TourLogs.Update(tourLog);
            await _context.SaveChangesAsync();
            return tourLog;
        }

        public async Task DeleteTourLogAsync(int tourLogId)
        {
            var tourLog = await _context.TourLogs.FindAsync(tourLogId);
            if (tourLog != null)
            {
                _context.TourLogs.Remove(tourLog);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<TourLog?> GetTourLogByIdAsync(int tourLogId)
        {
            return await _context.TourLogs.FindAsync(tourLogId);
        }
    }
}
