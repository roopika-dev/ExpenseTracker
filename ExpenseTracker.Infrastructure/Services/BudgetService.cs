using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Core.Models;
using ExpenseTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Infrastructure.Services
{
    public class BudgetService : IBudgetService
    {
        private readonly ApplicationDbContext _context;

        public BudgetService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SetBudget(int userId, int month, int year, decimal amount)
        {
            var budget = await _context.Budgets
                .FirstOrDefaultAsync(x => x.UserId == userId && x.Month == month && x.Year == year);

            if (budget == null)
            {
                budget = new Budget
                {
                    UserId = userId,
                    Month = month,
                    Year = year,
                    LimitAmount = amount
                };

                _context.Budgets.Add(budget);
            }
            else
            {
                budget.LimitAmount = amount;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<decimal> GetBudget(int userId, int month, int year)
        {
            var budget = await _context.Budgets
                .FirstOrDefaultAsync(x => x.UserId == userId && x.Month == month && x.Year == year);

            return budget?.LimitAmount ?? 0;
        }
    }
}