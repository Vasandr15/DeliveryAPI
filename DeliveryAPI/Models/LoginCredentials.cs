using System.ComponentModel.DataAnnotations;

namespace DeliveryAPI.Models;

public class LoginCredentials
{
    [Required]
    [MinLength(1)]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    [MinLength(6)]
    
    public string Password { get; set; }
}