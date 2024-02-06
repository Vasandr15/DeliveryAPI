using DeliveryAPI.DTO;
using DeliveryAPI.Models.DTO;
using DeliveryAPI.Models.Enums;
using DeliveryAPI.Services;
using DeliveryAPI.Services.DishService;
using DeliveryAPI.Services.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryAPI.Controllers;

[ApiController]
[Route("api/dish")]
public class DishController : Controller
{
    private readonly IDishService _dishService;
    private readonly IRepositoryService _repositoryService;

    public DishController(IDishService dishService, IRepositoryService repositoryService)
    {
        _dishService = dishService;
        _repositoryService = repositoryService;
    }

    [HttpGet]
    public async Task<DishPAgedListDto> GetAllDishes([FromQuery]DishCategory[]? categories = null, bool vegetarian = false,
        DishSorting? sorting= null, int? page = 1)
    {
        return await _dishService.GetAllDishes(categories, sorting, vegetarian, page ?? 1 );
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<DishDto> GetDishInfo(Guid id)
    {
        return await _dishService.GetDish(await _repositoryService.GetDish(id));
    }

    [HttpGet]
    [Route("{id}/rating/check")]
    [Authorize(policy:"AuthorizationPolicy")]
    public async Task<bool> CheckRating(Guid id)
    {
        return await _dishService.CheckRating(await _repositoryService.GetUser(User),
            await _repositoryService.GetDish(id));
    }

    [HttpPost]
    [Route("{id}/rating")]
    [Authorize(policy: "AuthorizationPolicy")]
    public async Task SetRating(Guid id, int ratingScore)
    {
        await _dishService.SetRating(await _repositoryService.GetUser(User),await _repositoryService.GetDish(id), ratingScore);
    }
}