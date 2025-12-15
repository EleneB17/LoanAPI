using LoanAPI.Data;
using LoanAPI.DTOs;
using LoanAPI.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace LoanAPI.Services.Loan;

public class LoanService : SLoanService
{
    private readonly LoanDbContext _context;

    public LoanService(LoanDbContext context)
    {
        _context = context;
    }

    public async Task<Models.Loan> CreateLoan(int userId, CreateLoanDto dto)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) throw new Exception("User not found");
        if (user.IsBlocked) throw new Exception("User is blocked");

        var loan = new Models.Loan
        {
            UserId = userId,
            Type = Enum.Parse<LoanType>(dto.Type),
            Amount = dto.Amount,
            Currency = Enum.Parse<Currency>(dto.Currency),
            PeriodMonths = dto.PeriodMonths
        };

        _context.Loans.Add(loan);
        await _context.SaveChangesAsync();

        Log.Information("Loan created:  LoanId={LoanId}, UserId={UserId}, Amount={Amount}",
            loan.Id, userId, dto.Amount);

        return loan;
    }

    public async Task<List<Models.Loan>> GetUserLoans(int userId)
    {
        return await _context.Loans.Where(l => l.UserId == userId).ToListAsync();
    }

    public async Task<Models.Loan> GetLoanById(int loanId)
    {
        var loan = await _context.Loans.Include(l => l.User).FirstOrDefaultAsync(l => l.Id == loanId);
        if (loan == null) throw new Exception("Loan not found");
        return loan;
    }

    public async Task<Models.Loan> UpdateLoan(int userId, int loanId, UpdateLoanDto dto)
    {
        var loan = await _context.Loans.FirstOrDefaultAsync(l => l.Id == loanId && l.UserId == userId);
        if (loan == null) throw new Exception("Loan not found");
        if (loan.Status != LoanStatus.Processing)
            throw new Exception("Only loans with 'Processing' status can be updated");

        if (dto.Type != null) loan.Type = Enum.Parse<LoanType>(dto.Type);
        if (dto.Amount.HasValue) loan.Amount = dto.Amount.Value;
        if (dto.Currency != null) loan.Currency = Enum.Parse<Currency>(dto.Currency);
        if (dto.PeriodMonths.HasValue) loan.PeriodMonths = dto.PeriodMonths.Value;

        await _context.SaveChangesAsync();
        Log.Information("Loan updated: LoanId={LoanId}", loanId);

        return loan;
    }

    public async Task DeleteLoan(int userId, int loanId)
    {
        var loan = await _context.Loans.FirstOrDefaultAsync(l => l.Id == loanId && l.UserId == userId);
        if (loan == null) throw new Exception("Loan not found");
        if (loan.Status != LoanStatus.Processing)
            throw new Exception("Only loans with 'Processing' status can be deleted");

        _context.Loans.Remove(loan);
        await _context.SaveChangesAsync();
        Log.Information("Loan deleted: LoanId={LoanId}", loanId);
    }

    public async Task<List<Models.Loan>> GetAllLoans()
    {
        return await _context.Loans.Include(l => l.User).ToListAsync();
    }

    public async Task UpdateLoanStatus(int loanId, UpdateLoanStatusDto dto)
    {
        var loan = await _context.Loans.FindAsync(loanId);
        if (loan == null) throw new Exception("Loan not found");

        loan.Status = Enum.Parse<LoanStatus>(dto.Status);
        await _context.SaveChangesAsync();
        Log.Information("Loan status updated: LoanId={LoanId}, NewStatus={Status}", loanId, dto.Status);
    }

    public async Task BlockUser(int userId, bool isBlocked)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) throw new Exception("User not found");

        user.IsBlocked = isBlocked;
        await _context.SaveChangesAsync();
        Log.Warning("User blocked status changed: UserId={UserId}, IsBlocked={IsBlocked}", userId, isBlocked);
    }

    public async Task DeleteLoanByAccountant(int loanId)
    {
        var loan = await _context.Loans.FindAsync(loanId);
        if (loan == null) throw new Exception("Loan not found");

        _context.Loans.Remove(loan);
        await _context.SaveChangesAsync();
        Log.Warning("Loan deleted by accountant:  LoanId={LoanId}", loanId);
    }
}