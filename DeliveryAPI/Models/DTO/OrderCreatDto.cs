using System.ComponentModel.DataAnnotations;

namespace DeliveryAPI.Models.DTO;

public class OrderCreatDto
{
    [Required]
    public DateTime DeliveryTime { get; set; }
    
    [Required]
    public Guid AddressId { get; set; }
}