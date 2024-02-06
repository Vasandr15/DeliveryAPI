using System.ComponentModel.DataAnnotations;
using DeliveryAPI.Models.Enums;

namespace DeliveryAPI.Models.DbEntities;

public class Dish
{
    public Guid Id { get; set; }
    
    public DishCategory Category { get; set; }
    
    [Required]
    [MinLength(1)]
    public string Name { get; set; }
    
    public string? Description { get; set; }
    
    [Required]
    [Range(0, double.MaxValue)]
    public double Price { get; set; }
    
    public double? Rating { get; set; }
    
    public string? Image { get; set; }
    
    public bool Vegetarian { get; set; }
    
    public ICollection<Rating>? Ratings { get; set; }
}