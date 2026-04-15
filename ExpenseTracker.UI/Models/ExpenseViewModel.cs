using System;

namespace ExpenseTracker.UI.Models
{
    public class ExpenseViewModel
    {
        public int Id { get; set; }

        public int CategoryId { get; set; }

        // ✅ ADD THIS (THIS IS YOUR FIX)
        public string CategoryName { get; set; }

        public decimal Amount { get; set; }

        public DateTime ExpenseDate { get; set; }

        public string Notes { get; set; }

        // 🔥 NEW
        public string Icon { get; set; }
        public string Color { get; set; }
    }
}