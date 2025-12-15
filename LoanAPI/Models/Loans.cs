namespace LoanAPI.Models;

public enum UserRole
{
    User,
    Accountant
}

public enum LoanType
{
    FastLoan,
    AutoLoan,
    Installment
}

public enum LoanStatus
{
    Processing,
    Approved,
    Rejected
}

public enum Currency
{
    GEL,
    USD,
    EUR
}