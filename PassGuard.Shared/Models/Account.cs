using System.ComponentModel.DataAnnotations;

namespace PassGuard.Shared.Models;

public class Account
{
    public Guid Id { get; set; }
    
    [Required]
    public string Username { get; set; }
    
    [Required]
    public string Password { get; set; }
    
    [Required]
    public string Salt { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
}