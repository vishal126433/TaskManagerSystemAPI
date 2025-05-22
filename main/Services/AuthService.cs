namespace TaskManager.Services
{
    public class AuthService
    {
        private readonly IAuthServiceClient _authServiceClient;

        public AuthService(IAuthServiceClient authServiceClient)
        {
            _authServiceClient = authServiceClient;
        }

        public Task<AuthResult> LoginAsync(LoginRequest request)
        {
            return _authServiceClient.LoginAsync(request);
        }
    }
}
