using DeliveryAPI.Models.DbEntities;
using DeliveryAPI.Models.DTO;

namespace DeliveryAPI.Services.OrderService;

public interface IOrderService
{
    public Task<OrderDto> GetOrderInfo(Order order);
    public Task<IEnumerable<OrderInfoDto>> GetAllOrders(User user);
    public Task CreateOrder(User user, OrderCreatDto orderCreatDto);
    public Task OrderDelivered(Order order);
}