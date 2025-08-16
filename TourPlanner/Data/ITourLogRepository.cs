using SWEN2_TourPlannerGroupProject.Models;

namespace SWEN2_TourPlannerGroupProject.Data
{
    public interface ITourLogRepository
    {
        Task<IEnumerable<TourLog>> GetTourLogsByTourIdAsync(int tourId);
        Task<TourLog> AddTourLogAsync(TourLog tourLog);
        Task<TourLog> UpdateTourLogAsync(TourLog tourLog);
        Task DeleteTourLogAsync(int tourLogId);
        Task<TourLog?> GetTourLogByIdAsync(int tourLogId);
    }
}
