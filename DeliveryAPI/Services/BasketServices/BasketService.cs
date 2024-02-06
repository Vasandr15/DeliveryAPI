using AutoMapper;
using DeliveryAPI.Exceptions;
using DeliveryAPI.Models.DbEntities;
using DeliveryAPI.Models.DTO;
using DeliveryAPI.Services.Repository;
using Microsoft.EntityFrameworkCore;

namespace DeliveryAPI.Services.BasketServices;

public class BasketService : IBasketService
{
    private readonly ApplicationDbContexts _contexts;
    private readonly IRepositoryService _repositoryService;
    private readonly IMapper _mapper;

    public BasketService(ApplicationDbContexts contexts, IRepositoryService repositoryService, IMapper mapper)
    {
        _contexts = contexts;
        _repositoryService = repositoryService;
        _mapper = mapper;
    }

    public async Task<List<DishBasketDto>> GetBasket(User user)
    {
        var dishBaskets = await _contexts.DishBaskets
            .Where(x => x.UserId == user.Id)
            .Include(x => x.Dish)
            .ToListAsync();

        var basketDtos = dishBaskets
            .Select(dishBasket => _mapper.Map<DishBasketDto>(dishBasket))
            .ToList();

        return basketDtos;
    }


    public async Task AddDishToCart(User user, Dish dish)
    {
        var cart = await _repositoryService.GetBasket(user.Id, dish.Id);
        if (cart != null) cart.Amount++;
        else
        {
            _contexts.DishBaskets.Add(new DishInCart()
            {
                Id = new Guid(),
                Amount = 1,
                UserId = user.Id,
                Dish = dish
            });
        }

        await _contexts.SaveChangesAsync();
    }

    public async Task DeleteDishFromCart(User user, Dish dish, bool increase = false)
    {
        var cart = await _repositoryService.GetBasket(user.Id, dish.Id);
        if (cart == null) throw new NotFoundException($"Dish with id={dish.Id} is not in basket");
        if (!increase || cart.Amount == 1)  _contexts.DishBaskets.Remove(cart);
        else
        {
            cart.Amount--;
        }
        
        await _contexts.SaveChangesAsync();
    }
}