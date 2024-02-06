using DeliveryAPI.Models.DbEntities;
using DeliveryAPI.Models.DTO;

namespace DeliveryAPI.Services.BasketServices;

public interface IBasketService
{
    public Task<List<DishBasketDto>> GetBasket(User user);
    public Task AddDishToCart(User user, Dish dish);
    public Task DeleteDishFromCart(User user, Dish dish, bool increase);
}