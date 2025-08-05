using Example_POS.DTOs.Token;
using Example_POS.DTOs.User;
using Example_POS.Models;
using System.Security.Claims;

namespace Example_POS.Services.Interfaces
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(RegisterDTO request);
        Task<TokenResponseDTO?> LoginAsync(LoginDTO request);
        ClaimsPrincipal? ValidateAccessToken(string token);
        Task<TokenResponseDTO?> ValidateRefreshToken(string refreshtoken);
    }
}
