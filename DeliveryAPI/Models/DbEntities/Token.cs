using System.ComponentModel.DataAnnotations;

namespace DeliveryAPI.Models.DbEntities;

public class Token
{
    public Guid Id { get; set; }
    
    [Required]
    public string ValidToken { get; set; }
    
    [Required]
    public DateTime ExpiredDate { get; set; }
    
}