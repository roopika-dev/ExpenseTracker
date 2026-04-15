using ExpenseTracker.Core.DTOs.Expense;
using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Core.Models;
using ExpenseTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Infrastructure.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly ApplicationDbContext _context;

        public ExpenseService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ExpenseResponseDto>> GetAll(int userId)
        {
            return await _context.Expenses
                .Where(x => x.UserId == userId)
                .Include(x => x.Category) // ✅ IMPORTANT
                .Select(x => new ExpenseResponseDto
                {
                    Id = x.Id,
                    CategoryId = x.CategoryId,            // ✅ FIX
                    CategoryName = x.Category.Name,       // ✅ FIX
                    Amount = x.Amount,
                    ExpenseDate = x.ExpenseDate,
                    Notes = x.Notes
                })
                .ToListAsync();
        }

        public async Task Add(int userId, CreateExpenseDto dto)
        {
            var expense = new Expense
            {
                UserId = userId,
                CategoryId = dto.CategoryId,
                Amount = dto.Amount,
                ExpenseDate = dto.ExpenseDate,
                Notes = dto.Notes
            };

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();
        }

        public async Task Update(int userId, UpdateExpenseDto dto)
        {
            var expense = await _context.Expenses
                .FirstOrDefaultAsync(x => x.Id == dto.Id && x.UserId == userId);

            if (expense == null) return;

            expense.CategoryId = dto.CategoryId;
            expense.Amount = dto.Amount;
            expense.ExpenseDate = dto.ExpenseDate;
            expense.Notes = dto.Notes;

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int userId, int expenseId)
        {
            var expense = await _context.Expenses
                .FirstOrDefaultAsync(x => x.Id == expenseId && x.UserId == userId);

            if (expense == null) return;

            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();
        }
    }
}