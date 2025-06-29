﻿using AuthService.Data;
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

    //public async Task<string> RegisterAsync(RegisterRequest request)
    //{
    //    var response = await _http.PostAsJsonAsync("https://localhost:7027/auth/register", request);
    //    if (response.IsSuccessStatusCode)
    //        return await response.Content.ReadAsStringAsync();
    //    throw new Exception("Registration failed");
    //}

    //public async Task<TokenResponse> LoginAsync(LoginRequest request)
    //{
    //    var response = await _http.PostAsJsonAsync("https://localhost:7027/auth/login", request);

    //    if (!response.IsSuccessStatusCode)
    //        throw new Exception("Invalid login credentials");

    //    var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();

    //    //  THIS SET-COOKIE HEADER is not passed to browser automatically
    //    var setCookieHeader = response.Headers.TryGetValues("Set-Cookie", out var cookies)
    //        ? cookies.FirstOrDefault()
    //        : null;

    //    // Return both token and cookie to controller
    //    return new TokenResponse
    //    {
    //        AccessToken = tokenResponse.AccessToken,
    //        RefreshToken = ExtractRefreshToken(setCookieHeader)
    //    };
    //}

    //private string ExtractRefreshToken(string setCookieHeader)
    //{
    //    if (string.IsNullOrEmpty(setCookieHeader)) return null;

    //    // Simple string extraction
    //    var parts = setCookieHeader.Split(';');
    //    var tokenPart = parts.FirstOrDefault(p => p.Trim().StartsWith("refreshToken="));
    //    return tokenPart?.Substring("refreshToken=".Length);
    //}



    public async Task<TokenResponse> RefreshTokenAsync(string refreshToken)
    {
        var message = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7027/auth/refresh-token");
        message.Headers.Add("Cookie", $"refreshToken={refreshToken}");

        var response = await _http.SendAsync(message);
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<TokenResponse>();
        throw new Exception("Refresh token failed");
    }

    //public async Task<string> LogoutAsync()
    //{
    //    var message = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7027/auth/logout");
    //    var response = await _http.SendAsync(message);
    //    return await response.Content.ReadAsStringAsync();
    //}
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

