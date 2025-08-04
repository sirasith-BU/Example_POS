using Example_POS.DTOs.User;
using Example_POS.Models;

namespace Example_POS.Services.Interfaces
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(RegisterDTO request);
        Task<string?> LoginAsync(LoginDTO request);
    }
}
