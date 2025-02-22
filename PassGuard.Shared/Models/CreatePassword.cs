namespace PassGuard.Shared.Models;

public class CreatePassword
{
    public string? Token { get; set; }
    public ObjectPasswordForm ObjectPasswordForm { get; set; }
}