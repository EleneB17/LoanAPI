using LoanAPI.DTOs;
using LoanAPI. Services.Loan;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoanAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Accountant")]
public class AccountantController : ControllerBase
{
    private readonly SLoanService _loanService;

    public AccountantController(SLoanService loanService)
    {
        _loanService = loanService;
    }

    [HttpGet("loans")]
    public async Task<IActionResult> GetAllLoans()
    {
        var loans = await _loanService.GetAllLoans();
        return Ok(loans);
    }

    [HttpPatch("loans/{id}/status")]
    public async Task<IActionResult> UpdateLoanStatus(int id, [FromBody] UpdateLoanStatusDto dto)
    {
        try
        {
            await _loanService.UpdateLoanStatus(id, dto);
            return Ok(new { message = "Status updated successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("users/{id}/block")]
    public async Task<IActionResult> BlockUser(int id, [FromBody] bool isBlocked)
    {
        try
        {
            await _loanService.BlockUser(id, isBlocked);
            return Ok(new { message = "User status updated successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("loans/{id}")]
    public async Task<IActionResult> DeleteLoan(int id)
    {
        try
        {
            await _loanService.DeleteLoanByAccountant(id);
            return Ok(new { message = "Loan deleted successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}