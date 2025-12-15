namespace LoanAPI.DTOs;
public class UpdateLoanDto
{
    public string? Type { get; set; }
    public decimal? Amount { get; set; }
    public string? Currency { get; set; }
    public int? PeriodMonths { get; set; }
}