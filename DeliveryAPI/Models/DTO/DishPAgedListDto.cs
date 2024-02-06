using System.ComponentModel.DataAnnotations;
using DeliveryAPI.DTO;

namespace DeliveryAPI.Models.DTO;

public class DishPAgedListDto
{
    [Required]
    public List<DishDto> Dishes { get; set; }
    
    [Required]
    public PageInfoModel Pagination { get; set; }
}