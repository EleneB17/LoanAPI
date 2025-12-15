using LoanAPI.DTOs;

namespace LoanAPI.Services.Auth;

public interface SAuthService
{
    Task<object> Register(RegisterDto dto);
    Task<object> Login(LoginDto dto);
}