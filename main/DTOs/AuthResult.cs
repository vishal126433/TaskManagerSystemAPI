public class AuthResult
{
    public bool IsSuccess { get; set; }
    public string Token { get; set; }    // or whatever you return
    public string Message { get; set; }
}
