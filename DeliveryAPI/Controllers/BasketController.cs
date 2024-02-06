using System.ComponentModel.DataAnnotations;
using DeliveryAPI.DTO;
using DeliveryAPI.Models.DTO;
using DeliveryAPI.Services.BasketServices;
using DeliveryAPI.Services.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryAPI.Controllers;

[ApiController]
[Route("api/basket")]
public class BasketController : Controller
{
    private readonly IBasketService _basketService;
    private readonly IRepositoryService _repositoryService;


    public BasketController(IBasketService basketService, IRepositoryService repositoryService)
    {
        _basketService = basketService;
        _repositoryService = repositoryService;
    }

    [HttpGet]
    [Authorize(policy: "AuthorizationPolicy")]
    public async Task<List<DishBasketDto>> GetBasket()
    {
        return await _basketService.GetBasket(await _repositoryService.GetUser(User));
    }

    [HttpPost]
    [Route("dish/{dishId}")]
    [Authorize(policy: "AuthorizationPolicy")]
    public async Task AddDishToCart([Required]Guid dishId)
    {
        await _basketService.AddDishToCart( await _repositoryService.GetUser(User), await _repositoryService.GetDish(dishId));
    }

    [HttpDelete]
    [Authorize(policy: "AuthorizationPolicy")]
    [Route("dish/{dishId}")]
    public async Task DeleteDishFromBasket([Required]Guid dishId, bool increase = false)
    { 
        await _basketService.DeleteDishFromCart(  await _repositoryService.GetUser(User),
            await _repositoryService.GetDish(dishId), increase);
    }
}