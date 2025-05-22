using AuthService.Models;

public interface IUserService
{
    Task<string> RegisterAsync(RegisterRequest request);
    Task<TokenResponse> LoginAsync(LoginRequest request);
    Task<TokenResponse> RefreshTokenAsync(string refreshToken);
    Task<string> LogoutAsync();
    Task<IEnumerable<User>> GetAllUsersAsync();

}
