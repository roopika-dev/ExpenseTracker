public class ExpenseResponseDto
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    public string CategoryName { get; set; }   // ✅ ADD THIS

    public decimal Amount { get; set; }

    public DateTime ExpenseDate { get; set; }

    public string Notes { get; set; }
}