using FluentValidation;
using LoanAPI.DTOs;

namespace LoanAPI.Validators;

public class RegisterValidator : AbstractValidator<RegisterDto>
{
    public RegisterValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("First name is required");
        RuleFor(x => x.LastName).NotEmpty().WithMessage("Last name is required");
        RuleFor(x => x.Username).NotEmpty().MinimumLength(3).WithMessage("Username must be at least 3 characters");
        RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Invalid email format");
        RuleFor(x => x.Age).InclusiveBetween(18, 100).WithMessage("Age must be between 18 and 100");
        RuleFor(x => x.Salary).GreaterThan(0).WithMessage("Monthly income must be positive");
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6).WithMessage("Password must be at least 6 characters");
    }
}