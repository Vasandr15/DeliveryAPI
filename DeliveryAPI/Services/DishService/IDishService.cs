using DeliveryAPI.Models.DbEntities;
using DeliveryAPI.Models.DTO;
using DeliveryAPI.Models.Enums;

namespace DeliveryAPI.Services.DishService;

public interface IDishService
{
    public Task<DishPAgedListDto> GetAllDishes(DishCategory[]? dishCategory, DishSorting? dishSorting, bool vegetarian,
        int page);
    public Task<DishDto> GetDish(Dish dish);
    public Task<bool> CheckRating(User user, Dish dish);
    public Task SetRating(User user, Dish dish, int ratingScore);
}