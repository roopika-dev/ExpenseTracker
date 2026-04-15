namespace ExpenseTracker.Core.Models
{
    public class Expense
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public int CategoryId { get; set; }

        public decimal Amount { get; set; }

        public DateTime ExpenseDate { get; set; }

        public string Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        public User User { get; set; }
        public Category Category { get; set; }
    }
}