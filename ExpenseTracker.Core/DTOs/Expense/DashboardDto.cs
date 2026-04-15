using ExpenseTracker.Core.DTOs.Expense;

namespace ExpenseTracker.Core.DTOs.Dashboard
{
    public class DashboardDto
    {
        public decimal TotalExpense { get; set; }
        public string TopCategory { get; set; }
        public List<ExpenseResponseDto> RecentExpenses { get; set; }
    }
}