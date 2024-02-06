using System.Security.Claims;
using DeliveryAPI.Models.DbEntities;

namespace DeliveryAPI.Services.Repository;

public interface IRepositoryService
{
    Task<User> GetUser(ClaimsPrincipal claimsPrincipal);
    Task<Dish> GetDish(Guid id);
    Task<DishInCart?> GetBasket(Guid userId, Guid dishId);
    Task<Order> GetOrder(User user, Guid orderId);
}