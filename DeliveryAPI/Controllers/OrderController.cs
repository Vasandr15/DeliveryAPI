using DeliveryAPI.DTO;
using DeliveryAPI.Models.DTO;
using DeliveryAPI.Services.OrderService;
using DeliveryAPI.Services.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryAPI.Controllers;

[ApiController]
[Route("api/order")]
public class OrderController : Controller
{
    private readonly IRepositoryService _repositoryService;
    private readonly IOrderService _orderService;

    public OrderController(IRepositoryService repositoryService, IOrderService orderService)
    {
        _repositoryService = repositoryService;
        _orderService = orderService;
    }

    [HttpGet]
    [Route("{id}")]
    [Authorize(policy: "AuthorizationPolicy")]
    public async Task<OrderDto> GetOrderInfo(Guid id)
    {
        return await _orderService.GetOrderInfo(await _repositoryService.GetOrder(await _repositoryService.GetUser(User), id));
    }

    [HttpGet]
    [Authorize(policy: "AuthorizationPolicy")]
    public async Task<IEnumerable<OrderInfoDto>> GetAllOrders()
    {
        return await _orderService.GetAllOrders(await _repositoryService.GetUser(User));
    }
    
    [HttpPost]
    [Authorize(policy: "AuthorizationPolicy")]
    public async Task CreateOrder(OrderCreatDto orderCreatDto)
    {
        await _orderService.CreateOrder(await _repositoryService.GetUser(User), orderCreatDto);
    }

    [HttpPost]
    [Route("{id}/status")]
    [Authorize(policy: "AuthorizationPolicy")]
    public async Task OrderDelivered(Guid id)
    {
        await _orderService.OrderDelivered(await _repositoryService.GetOrder(await _repositoryService.GetUser(User), id));
    }
}