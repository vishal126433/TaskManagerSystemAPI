using Microsoft.AspNetCore.Identity.Data;
using System.Net.Http.Json;

public class AuthServiceClient : IAuthServiceClient
{
    private readonly HttpClient _httpClient;

    public AuthServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("users/login", request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<AuthResult>();
    }
}
