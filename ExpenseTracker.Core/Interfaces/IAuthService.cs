using ExpenseTracker.Core.DTOs.Auth;

namespace ExpenseTracker.Core.Interfaces
{
    public interface IAuthService
    {
        Task<string> Register(RegisterDto dto);
        Task<string> Login(LoginDto dto);
    }
}