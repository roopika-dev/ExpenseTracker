using ExpenseTracker.Core.DTOs.Dashboard;

namespace ExpenseTracker.Core.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardDto> GetDashboard(int userId);
    }
}