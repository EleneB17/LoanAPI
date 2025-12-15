using LoanAPI.DTOs;
using LoanAPI. Services.Loan;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore. Mvc;
using System.Security. Claims;

namespace LoanAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class LoanController : ControllerBase
{
    private readonly SLoanService _loanService;

    public LoanController(SLoanService loanService)
    {
        _loanService = loanService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateLoan([FromBody] CreateLoanDto dto)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var loan = await _loanService.CreateLoan(userId, dto);
            return Ok(loan);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("my-loans")]
    public async Task<IActionResult> GetMyLoans()
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var loans = await _loanService.GetUserLoans(userId);
            return Ok(loans);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetLoan(int id)
    {
        try
        {
            var loan = await _loanService.GetLoanById(id);
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (role != "Accountant" && loan.UserId != userId)
                return Forbid();

            return Ok(loan);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLoan(int id, [FromBody] UpdateLoanDto dto)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var loan = await _loanService.UpdateLoan(userId, id, dto);
            return Ok(loan);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLoan(int id)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            await _loanService.DeleteLoan(userId, id);
            return Ok(new { message = "Loan deleted successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}