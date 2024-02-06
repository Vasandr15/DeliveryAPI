using System.ComponentModel.DataAnnotations;
using DeliveryAPI.Models.Enums;

namespace DeliveryAPI.Models;

public class UserRegisterModel
{
    [Required]
    [MinLength(1)]
    public string FullName { get; set; }
    
    [Required]
    [MinLength(6)]
    public string Password { get; set; }
    
    [Required]
    [MinLength(1)]
    [EmailAddress]
    public string Email { get; set; }
    
    public Guid? AddressId { get; set; }
    
    public DateTime? BirthDate { get; set; }

    [Required] 
    public Gender Gender { get; set; }
    
    [Phone]
    public string? PhoneNumber { get; set; }
}