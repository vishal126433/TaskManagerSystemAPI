using AuthService.Data;
using AuthService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace TaskManager.Services.Users;

public class UserService : IUserService
{
    private readonly HttpClient _http;
    private readonly AppDbContext _context;

    public UserService(HttpClient http, AppDbContext context)
    {
        _http = http;
        _context = context;
    }
    public async Task<TokenResponse> RefreshTokenAsync(string refreshToken)
    {
        var message = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7027/auth/refresh-token");
        message.Headers.Add("Cookie", $"refreshToken={refreshToken}");

        var response = await _http.SendAsync(message);
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<TokenResponse>();
        throw new Exception("Refresh token failed");
    }

   
    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }


    public async Task<User> CreateUserAsync(RegisterRequest request)
    {
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

        return user;
    }

    public async Task<User?> UpdateUserAsync(int id, User updatedUser)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return null;

        user.Username = updatedUser.Username;
        user.Email = updatedUser.Email;
        user.PasswordHash = updatedUser.PasswordHash;
        user.Role = updatedUser.Role;

        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }

}

