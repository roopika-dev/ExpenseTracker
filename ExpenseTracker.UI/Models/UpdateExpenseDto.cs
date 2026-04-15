using System;

namespace ExpenseTracker.UI.Models
{
    public class UpdateExpenseDto
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string Notes { get; set; }
    }
}