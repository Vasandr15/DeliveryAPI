using System.ComponentModel.DataAnnotations;
using DeliveryAPI.Models.Enums;

namespace DeliveryAPI.Models.DbEntities;

public class User
{
    public Guid Id { get; set; }
    
    [MinLength(1)]
    [Required]
    public string FullName { get; set; }
    
    public DateTime? BirthDate { get; set; }
        
    [Required]
    public Gender Gender { get; set; }
    
    public Guid? AddressId { get; set; }
    
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    [MinLength(6)]
    public string Password { get; set; }
    
    public string? PhoneNumber { get; set; }
    
    public ICollection<Order> Orders { get; set; }
    public ICollection<DishInCart> Cart { get; set; }
    public ICollection<Rating> Ratings { get; set; }
}