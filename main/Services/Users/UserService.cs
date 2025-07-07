using AuthService.Data;
using AuthService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace TaskManager.Services.Users
{
    public class UserService : IUserService
    {
        private readonly HttpClient _http;
        private readonly AppDbContext _context;
        private readonly ILogger<UserService> _logger;

        public UserService(HttpClient http, AppDbContext context, ILogger<UserService> logger)
        {
            _http = http ?? throw new ArgumentNullException(nameof(http), "HttpClient cannot be null.");
            _context = context ?? throw new ArgumentNullException(nameof(context), "DbContext cannot be null.");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger cannot be null.");
        }

        public async Task<TokenResponse> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                _logger.LogInformation("Refreshing token for provided refresh token");

                var message = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7027/auth/refresh-token");
                message.Headers.Add("Cookie", $"refreshToken={refreshToken}");

                var response = await _http.SendAsync(message);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Refresh token succeeded");
                    return await response.Content.ReadFromJsonAsync<TokenResponse>();
                }

                _logger.LogWarning("Refresh token failed with status code {StatusCode}", response.StatusCode);
                throw new Exception("Refresh token failed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing token");
                throw;
            }
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all users");
                var users = await _context.Users.ToListAsync();
                _logger.LogInformation("Fetched {Count} users", users.Count);
                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all users");
                throw;
            }
        }

        public async Task<User> CreateUserAsync(RegisterRequest request)
        {
            try
            {
                _logger.LogInformation("Attempting to create user with username '{Username}'", request.Username);

                var passwordHasher = new PasswordHasher<User>();

                var user = new User
                {
                    Username = request.Username,
                    Email = request.Email,
                    Role = string.IsNullOrWhiteSpace(request.Role) ? "User" : request.Role
                };

                user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User created successfully with Id {UserId}", user.Id);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user with username '{Username}'", request.Username);
                throw;
            }
        }

        public async Task<User?> UpdateUserAsync(int id, User updatedUser)
        {
            try
            {
                _logger.LogInformation("Attempting to update user with Id {UserId}", id);

                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("Update failed: user with Id {UserId} not found", id);
                    return null;
                }

                user.Username = updatedUser.Username;
                user.Email = updatedUser.Email;
                user.PasswordHash = updatedUser.PasswordHash;
                user.Role = updatedUser.Role;

                await _context.SaveChangesAsync();

                _logger.LogInformation("User with Id {UserId} updated successfully", id);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with Id {UserId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            try
            {
                _logger.LogInformation("Attempting to delete user with Id {UserId}", id);

                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("Delete failed: user with Id {UserId} not found", id);
                    return false;
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User with Id {UserId} deleted successfully", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with Id {UserId}", id);
                throw;
            }
        }
    }
}
