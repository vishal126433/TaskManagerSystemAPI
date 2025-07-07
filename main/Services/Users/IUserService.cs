using AuthService.Models;

namespace TaskManager.Services.Users;

public interface IUserService
{
    
    Task<TokenResponse> RefreshTokenAsync(string refreshToken);
    /// <summary>
    /// code to create new user
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<User> CreateUserAsync(RegisterRequest request);
    /// <summary>
    /// code to update the details of a specific user
    /// </summary>
    /// <param name="id"></param>
    /// <param name="updatedUser"></param>
    /// <returns></returns>
    Task<User> UpdateUserAsync(int id, User updatedUser);
    /// <summary>
    /// code to delete a specific user
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<bool> DeleteUserAsync(int id);
    /// <summary>
    /// code to display the list of all users
    /// </summary>
    /// <returns></returns>

    Task<IEnumerable<User>> GetAllUsersAsync();
    
}
