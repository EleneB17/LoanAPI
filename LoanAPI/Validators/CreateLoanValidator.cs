using FluentValidation;
using LoanAPI. DTOs;

namespace LoanAPI.Validators;

public class CreateLoanValidator : AbstractValidator<CreateLoanDto>
{
    public CreateLoanValidator()
    {
        RuleFor(x => x.Type)
            .NotEmpty()
            .Must(x => x == "FastLoan" || x == "AutoLoan" || x == "Installment")
            .WithMessage("Loan type must be:  FastLoan, AutoLoan, or Installment");

        RuleFor(x => x.Amount)
            .InclusiveBetween(100, 1000000)
            .WithMessage("Amount must be between 100 and 1,000,000");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .Must(x => x == "GEL" || x == "USD" || x == "EUR")
            .WithMessage("Currency must be: GEL, USD, or EUR");

        RuleFor(x => x.PeriodMonths)
            .InclusiveBetween(1, 360)
            .WithMessage("Period must be between 1 and 360 months");
    }
}