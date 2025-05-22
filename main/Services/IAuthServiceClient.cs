using Microsoft.AspNetCore.Identity.Data;

public interface IAuthServiceClient
{
    Task<AuthResult> LoginAsync(LoginRequest request);
}
