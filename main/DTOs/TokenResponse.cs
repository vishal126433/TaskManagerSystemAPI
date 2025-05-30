public class TokenResponse
{
    public string AccessToken { get; set; }
    public string Role { get; set; }
    public string RefreshToken { get; internal set; }
}
