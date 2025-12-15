using LoanAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LoanAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UserController : ControllerBase
{
    private readonly LoanDbContext _context;

    public UserController(LoanDbContext context)
    {
        _context = context;
    }

    [HttpGet("{id}")]
    public IActionResult GetUser(int id)
    {
        var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        if (role != "Accountant" && currentUserId != id)
            return Forbid();

        var user = _context.Users.Find(id);
        if (user == null)
            return NotFound(new { message = "User not found" });

        return Ok(user);
    }
}