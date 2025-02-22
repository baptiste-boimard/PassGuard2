using System.ComponentModel.DataAnnotations;

namespace PassGuard.Shared.Models;

public class UpdatePassword
{
    public Guid Id { get; set; }
    
    [Required]
    public string Site { get; set; }
    
    [Required]
    public string Username { get; set; }
    
    [Required]
    public string Password { get; set; }
    
    [Required]
    public string Category { get; set; }
    
    public DateTime CreatedAt { get; set; }
}