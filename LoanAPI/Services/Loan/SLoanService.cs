using LoanAPI.DTOs;
using LoanAPI.Models;

namespace LoanAPI.Services.Loan;

public interface SLoanService
{
    Task<Models.Loan> CreateLoan(int userId, CreateLoanDto dto);
    Task<List<Models.Loan>> GetUserLoans(int userId);
    Task<Models.Loan> GetLoanById(int loanId);
    Task<Models.Loan> UpdateLoan(int userId, int loanId, UpdateLoanDto dto);
    Task DeleteLoan(int userId, int loanId);
    Task<List<Models.Loan>> GetAllLoans();
    Task UpdateLoanStatus(int loanId, UpdateLoanStatusDto dto);
    Task BlockUser(int userId, bool isBlocked);
    Task DeleteLoanByAccountant(int loanId);
}