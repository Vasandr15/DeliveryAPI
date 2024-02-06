using System.ComponentModel.DataAnnotations;

namespace DeliveryAPI.Models.DbEntities;

public class Rating
{
    public Guid Id { get; set; }
    
    public Dish Dish { get; set; }
    
    public User? User { get; set; }
    
    [Range(0,5)]
    public int Value { get; set; }
}