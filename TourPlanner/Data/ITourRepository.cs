using SWEN2_TourPlannerGroupProject.Models;

namespace SWEN2_TourPlannerGroupProject.Data
{
    public interface ITourRepository
    {
        Task<IEnumerable<Tour>> GetAllToursAsync();
        Task<Tour> AddTourAsync(Tour tour);
        Task DeleteTourAsync(int id);
        Task UpdateTourAsync(Tour tour);
    }
} 