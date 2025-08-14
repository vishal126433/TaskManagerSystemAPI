using TaskManager.Data;
using TaskManager.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManager.Helpers;
using TaskManager.Interfaces;

namespace TaskManager.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _http;
        private readonly AppDbContext _context;
        private readonly ILogger<UserService> _logger;

        public UserService(HttpClient http, AppDbContext context, ILogger<UserService> logger)
        {
            _http = http ?? throw new ArgumentNullException(nameof(http), ResponseMessages.Message.HttpClientNull);
            _context = context ?? throw new ArgumentNullException(nameof(context), ResponseMessages.Message.DBcontextNull);
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), ResponseMessages.Message.LoggerNull);
        }

        public async Task<TokenResponse> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                _logger.LogInformation(ResponseMessages.Message.RefreshingToken);

                var message = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7027/auth/refresh-token");
                message.Headers.Add("Cookie", $"refreshToken={refreshToken}");

                var response = await _http.SendAsync(message);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation(ResponseMessages.Message.TokenSucceed);
                    return await response.Content.ReadFromJsonAsync<TokenResponse>();
                }

                _logger.LogWarning(ResponseMessages.Message.TokenFailedcode, response.StatusCode);
                throw new Exception(ResponseMessages.Message.TokenFailed);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ResponseMessages.Message.TokenError);
                throw;
            }
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            try
            {
                _logger.LogInformation(ResponseMessages.Message.FetchingUser);
                var users = await _context.Users.ToListAsync();
                _logger.LogInformation(ResponseMessages.Message.UsersFetched, users.Count);
                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ResponseMessages.Message.ErrorUserFetching);
                throw;
            }
        }

        public async Task<User> CreateUserAsync(RegisterRequest request)
        {
            try
            {
                _logger.LogInformation(ResponseMessages.Message.AttemptCreateUser, request.Username);

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

                _logger.LogInformation(ResponseMessages.Message.CreateUserWithId, user.Id);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ResponseMessages.Message.CreateUserError, request.Username);
                throw;
            }
        }

        public async Task<User?> UpdateUserAsync(int id, User updatedUser)
        {
            try
            {
                _logger.LogInformation(ResponseMessages.Message.AttemptUpdateUser, id);

                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    _logger.LogWarning(ResponseMessages.Message.UpdateUserFailed, id);
                    return null;
                }

                user.Username = updatedUser.Username;
                user.Email = updatedUser.Email;
                user.PasswordHash = updatedUser.PasswordHash;
                user.Role = updatedUser.Role;

                await _context.SaveChangesAsync();

                _logger.LogInformation(ResponseMessages.Message.UserUpdatedLog, id);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,ResponseMessages.Message.UserUpdatedErrorLog, id);
                throw;
            }
        }
        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }


        public async Task<bool> DeleteUserAsync(int id)
        {
            try
            {
                _logger.LogInformation(ResponseMessages.Message.AttemptDeleteUser, id);

                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    _logger.LogWarning(ResponseMessages.Message.DeleteUserFailed, id);
                    return false;
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation(ResponseMessages.Message.UserDeletedLog, id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ResponseMessages.Message.UserDeletedErrorLog, id);
                throw;
            }
        }
        public async Task<bool?> ToggleUserActiveStatusAsync(int id)
        {
            try
            {
                _logger.LogInformation(ResponseMessages.Message.ToggleActiveStatus, id);

                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    _logger.LogWarning(ResponseMessages.Message.UserIdNotFound, id);
                    return null;
                }

                user.IsActive = !user.IsActive;
                await _context.SaveChangesAsync();

                _logger.LogInformation(ResponseMessages.Message.UserIdActiveStatus, id, user.IsActive);
                return user.IsActive;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ResponseMessages.Message.ToggleActiveStatusError, id);
                throw;
            }
        }

    }
}
