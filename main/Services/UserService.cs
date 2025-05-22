using AuthService.Data;
using AuthService.Models;
using Microsoft.EntityFrameworkCore;


    public class UserService : IUserService
    {
        private readonly HttpClient _http;
        private readonly AppDbContext _context;

        public UserService(HttpClient http, AppDbContext context)
        {
            _http = http;
            _context = context;
        }

    public async Task<string> RegisterAsync(RegisterRequest request)
    {
        var response = await _http.PostAsJsonAsync("https://localhost:7027/auth/register", request);
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadAsStringAsync();
        throw new Exception("Registration failed");
    }

    public async Task<TokenResponse> LoginAsync(LoginRequest request)
    {
        var response = await _http.PostAsJsonAsync("https://localhost:7027/auth/login", request);
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<TokenResponse>();
        throw new Exception("Invalid login credentials");
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

    public async Task<string> LogoutAsync()
    {
        var message = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7027/auth/logout");
        var response = await _http.SendAsync(message);
        return await response.Content.ReadAsStringAsync();
    }
    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }






}
