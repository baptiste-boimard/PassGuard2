namespace PassGuard.Shared.Models;

public class VerifyPassword
{
    public string Password { get; set; }
    public string Token { get; set; }
    public Guid IdLine { get; set; }
}