using System.Security.Claims;
using DeliveryAPI.Exceptions;
using DeliveryAPI.Models.DbEntities;
using Microsoft.EntityFrameworkCore;

namespace DeliveryAPI.Services.Repository;

public class RepositoryService : IRepositoryService
{
    private readonly ApplicationDbContexts _contexts;

    public RepositoryService(ApplicationDbContexts contexts)
    {
        _contexts = contexts;
    }
    
    public async Task<User> GetUser(ClaimsPrincipal? claimsPrincipal)
    {
        var userId = Guid.Parse(claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var user = await _contexts.Users.SingleOrDefaultAsync(x => x.Id == userId);
        if (user == null)
        {
            throw new Unauthorized();
        }

        return user;
    }

    public async Task<Dish> GetDish(Guid id)
    {
        var dish = await _contexts.Dishes.SingleOrDefaultAsync(x => x.Id == id);
        if (dish == null)
        {
            throw new NotFoundException("Dish not found");
        }

        return dish;
    }

    public async Task<DishInCart?> GetBasket(Guid userId, Guid dishId)
    {
        return await _contexts.DishBaskets.SingleOrDefaultAsync(x => x != null && x.UserId == userId && x.Dish.Id == dishId);
    }

    public async  Task<Order> GetOrder(User user, Guid orderId)
    {
        var order = await _contexts.Orders.SingleOrDefaultAsync(x=> x.User == user && x.Id == orderId);
        if (order == null)
        {
            throw new NotFoundException("Order not found");
        }

        return order;
    }
}