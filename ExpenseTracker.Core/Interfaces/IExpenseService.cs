using ExpenseTracker.Core.DTOs.Expense;

namespace ExpenseTracker.Core.Interfaces
{
    public interface IExpenseService
    {
        Task<List<ExpenseResponseDto>> GetAll(int userId);
        Task Add(int userId, CreateExpenseDto dto);
        Task Update(int userId, UpdateExpenseDto dto);
        Task Delete(int userId, int expenseId);
    }
}