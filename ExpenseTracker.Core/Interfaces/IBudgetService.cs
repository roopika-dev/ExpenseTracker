namespace ExpenseTracker.Core.Interfaces
{
    public interface IBudgetService
    {
        Task SetBudget(int userId, int month, int year, decimal amount);
        Task<decimal> GetBudget(int userId, int month, int year);
    }
}