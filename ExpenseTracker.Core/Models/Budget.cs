namespace ExpenseTracker.Core.Models
{
    public class Budget
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int Month { get; set; }
        public int Year { get; set; }

        public decimal LimitAmount { get; set; }

        // Navigation
        public User User { get; set; }
    }
}