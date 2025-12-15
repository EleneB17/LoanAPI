using LoanAPI.Data;
using LoanAPI.DTOs;
using LoanAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Serilog;

namespace LoanAPI.Services.Auth;

public class AuthService : SAuthService
{
    private readonly LoanDbContext _context;
    private readonly IConfiguration _config;

    public AuthService(LoanDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task<object> Register(RegisterDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
            throw new Exception("Username already exists");

        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            throw new Exception("Email already in use");

        var user = new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Username = dto.Username,
            Email = dto.Email,
            Age = dto.Age,
            Salary = dto.Salary,
            PasswordHash = HashPassword(dto.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        Log.Information("User registered: {Username}", user.Username);

        return new
        {
            user.Id,
            user.Username,
            user.FirstName,
            Role = user.Role.ToString(),
            Token = GenerateToken(user)
        };
    }

    public async Task<object> Login(LoginDto dto)
    {
        if (string.IsNullOrEmpty(dto.Username) || string.IsNullOrEmpty(dto.Password))
            throw new Exception("Username and password are required");

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);

        if (user == null)
        {
            Log.Warning("Failed login attempt - user not found: {Username}", dto.Username);
            throw new Exception("Invalid username or password");
        }

        if (!VerifyPassword(dto.Password, user.PasswordHash))
        {
            Log.Warning("Failed login attempt - wrong password: {Username}", dto.Username);
            throw new Exception("Invalid username or password");
        }

        Log.Information("User logged in: {Username}", user.Username);

        return new
        {
            user.Id,
            user.Username,
            user.FirstName,
            Role = user.Role.ToString(),
            Token = GenerateToken(user)
        };
    }

    private string GenerateToken(User user)
    {
        var jwtKey = _config["Jwt:Key"];

        if (string.IsNullOrEmpty(jwtKey))
            throw new Exception("JWT Key is not configured");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            throw new ArgumentException("Password cannot be null or empty", nameof(password));

        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    private bool VerifyPassword(string password, string hash)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hash))
            return false;

        return HashPassword(password) == hash;
    }
}