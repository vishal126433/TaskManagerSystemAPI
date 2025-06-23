using AuthService.Models;

namespace TaskManager.Services.Users;

public interface IUserService
{
    //Task<string> RegisterAsync(RegisterRequest request);
    //Task<TokenResponse> LoginAsync(LoginRequest request);
    Task<TokenResponse> RefreshTokenAsync(string refreshToken);
    //Task<string> LogoutAsync();
    Task<User> CreateUserAsync(RegisterRequest request);
    Task<User> UpdateUserAsync(int id, User updatedUser);
    Task<bool> DeleteUserAsync(int id);

    Task<IEnumerable<User>> GetAllUsersAsync();

}
