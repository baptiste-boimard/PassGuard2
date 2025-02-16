namespace PassGuard.Shared.DTO;

public class ObjectPasswordDTO
{
    public Guid Id { get; set; }
    public string Site { get; set; } 
    public string Username { get; set; } 
    public string Password { get; set; } 
    public string Category { get; set; }
    public DateTime CreatedAt { get; set; }
}