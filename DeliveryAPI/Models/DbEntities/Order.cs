using System.ComponentModel.DataAnnotations;
using DeliveryAPI.Models.Enums;

namespace DeliveryAPI.Models.DbEntities;

public class Order
{
    public Guid Id { get; set; }
    
    [Required]
    public DateTime OrderTime { get; set; }
    
    [Required]
    public DateTime DeliveryTime { get; set; }
    
    [Required]
    public OrderStatus Status { get; set; }
    
    [Required]
    public double Price { get; set; }
    
    [Required]
    public ICollection<DishInCart> DishInCart { get; set; }
    
    [Required]
    [MinLength(1)]
    public Guid AddressId { get; set; }
    
    public User User { get; set; }
}