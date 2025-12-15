namespace LoanAPI.Models;

public class Loan
{
    public int Id { get; set; }
    public LoanType Type { get; set; }
    public decimal Amount { get; set; }
    public Currency Currency { get; set; }
    public int PeriodMonths { get; set; }
    public LoanStatus Status { get; set; } = LoanStatus.Processing;
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}