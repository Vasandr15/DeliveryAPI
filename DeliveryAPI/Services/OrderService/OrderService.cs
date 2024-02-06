using AutoMapper;
using DeliveryAPI.Helpers;
using DeliveryAPI.Exceptions;
using DeliveryAPI.Models.DbEntities;
using DeliveryAPI.Models.DTO;
using DeliveryAPI.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace DeliveryAPI.Services.OrderService;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContexts _contexts;
    private readonly IMapper _mapper;

    public OrderService(ApplicationDbContexts contexts, IMapper mapper)
    {
        _contexts = contexts;
        _mapper = mapper;
    }

    public async Task<OrderDto> GetOrderInfo(Order order)
    {
        var orderInfo = _mapper.Map<OrderDto>(order);
        var orderEntity = await _contexts.Orders
            .Include(o => o.DishInCart)  
            .ThenInclude(d => d.Dish)    
            .FirstOrDefaultAsync(o => o.Id == order.Id);

        orderInfo.Dishes = orderEntity.DishInCart.Select(dishes => _mapper.Map<DishBasketDto>(dishes)).ToList();
        return orderInfo;
    }

    public async Task<IEnumerable<OrderInfoDto>> GetAllOrders(User user)
    {
        return await _contexts.Orders.Where(x => x.User.Id == user.Id)
            .Select(x => new OrderInfoDto
            {
                Id = x.Id,
                DeliveryTime = x.DeliveryTime,
                OrderTime = x.OrderTime,
                Status = x.Status,
                Price = x.Price
            }).ToListAsync();
    }

    public async Task CreateOrder(User user, OrderCreatDto orderCreatDto)
    {
        ICollection<DishInCart> cart =  await _contexts.DishBaskets.Where(x => x != null && x.UserId == user.Id).
            Include(x => x.Dish).ToListAsync();

        if (cart == null)
        {
            throw new BadRequest($"Empty basket for user with id={user.Id}");
        }
        CheckInfoHelper.CheckDeliveryTime(orderCreatDto.DeliveryTime);
        var order = new Order()
        {
            Id = new Guid(),
            AddressId = orderCreatDto.AddressId,
            DishInCart = cart,
            DeliveryTime = orderCreatDto.DeliveryTime,
            OrderTime = DateTime.UtcNow,
            Status = OrderStatus.InProcess,
            Price = cart.Sum(x=>x.Amount*x.Dish.Price),
            User = user
        };
        await _contexts.Orders.AddAsync(order);
        foreach (var dishBasket in user.Cart)
        {
            dishBasket.UserId = null;
        }

        await _contexts.SaveChangesAsync();
    }

    public async Task OrderDelivered(Order order)
    {
        order.Status = OrderStatus.Delivered;
        await _contexts.SaveChangesAsync();
    }
}