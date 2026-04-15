namespace ExpenseTracker.Core.DTOs.Expense
{
    public class CreateExpenseDto
    {
        public int CategoryId { get; set; }
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string Notes { get; set; }
    }
}