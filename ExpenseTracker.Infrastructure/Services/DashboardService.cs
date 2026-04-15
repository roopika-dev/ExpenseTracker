using ExpenseTracker.Core.DTOs.Dashboard;
using ExpenseTracker.Core.DTOs.Expense;
using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Infrastructure.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _context;

        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardDto> GetDashboard(int userId)
        {
            var expenses = await _context.Expenses
                .Where(x => x.UserId == userId)
                .Include(x => x.Category)
                .ToListAsync();

            var total = expenses.Sum(x => x.Amount);

            var topCategory = expenses
                .GroupBy(x => x.Category.Name)
                .OrderByDescending(g => g.Sum(x => x.Amount))
                .Select(g => g.Key)
                .FirstOrDefault();

            var recent = expenses
                .OrderByDescending(x => x.ExpenseDate)
                .Take(5)
                .Select(x => new ExpenseResponseDto
                {
                    Id = x.Id,
                    CategoryName = x.Category.Name,
                    Amount = x.Amount,
                    ExpenseDate = x.ExpenseDate,
                    Notes = x.Notes
                }).ToList();

            return new DashboardDto
            {
                TotalExpense = total,
                TopCategory = topCategory,
                RecentExpenses = recent
            };
        }
    }
}