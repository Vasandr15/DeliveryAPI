using System.ComponentModel.DataAnnotations;

namespace DeliveryAPI.Models.DbEntities;

public class DishInCart
{
    public Guid Id { get; set; }
    
    [Range(0, double.MaxValue)]
    public int Amount { get; set; }
    
    public Guid? UserId { get; set; }
    
    public Dish Dish { get; set; }
    
    public Guid? OrderId { get; set; }
}