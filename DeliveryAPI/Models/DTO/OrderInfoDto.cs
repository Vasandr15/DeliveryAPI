using System.ComponentModel.DataAnnotations;
using DeliveryAPI.Models.Enums;

namespace DeliveryAPI.Models.DTO;

public class OrderInfoDto
{
    public Guid Id { get; set; }
    
    [Required]
    public DateTime DeliveryTime { get; set; }
    
    [Required]
    public DateTime OrderTime { get; set; }
    
    [Required]
    public OrderStatus Status { get; set; }
    
    [Required]
    [Range(0,double.MaxValue)]
    public double Price { get; set; }
}