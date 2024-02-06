using AutoMapper;
using DeliveryAPI.Exceptions;
using DeliveryAPI.Models;
using DeliveryAPI.Models.DbEntities;
using DeliveryAPI.Models.DTO;
using DeliveryAPI.Models.Enums;
using Microsoft.EntityFrameworkCore;
using static DeliveryAPI.Models.Enums.DishSorting;

namespace DeliveryAPI.Services.DishService;

public class DishServices : IDishService
{
    private readonly ApplicationDbContexts _contexts;
    private readonly IMapper _mapper;
    private readonly int _pageSize;

    public DishServices(ApplicationDbContexts contexts, IConfiguration configuration, IMapper mapper)
    {
        _pageSize = configuration.GetValue<int>("PageSize");
        _mapper = mapper;
        _contexts = contexts;
    }

    public Task<DishPAgedListDto> GetAllDishes(DishCategory[]? dishCategory, DishSorting? dishSorting, bool vegetarian, int page)
    {
       
        var dishes =  _contexts.Dishes.AsQueryable();
        dishes = GetVegetarian(dishes,vegetarian);
        dishes = GetCategory(dishes,dishCategory);
        dishes = SortDishes(dishes, dishSorting);

        if (dishes == null) throw new NotFoundException("No such dishes");
        var totalPages = (int)Math.Ceiling(((decimal)dishes.Count() / _pageSize));
        
        if (totalPages < page) throw new BadRequest("Invalid value for attribute page");
        
        var resultDishes = dishes.Skip(_pageSize * (page - 1)).Take(_pageSize).ToList();

        return Task.FromResult(new DishPAgedListDto()
        {
            Dishes = (resultDishes.Select(dish => _mapper.Map<DishDto>(dish)).ToList()),
            Pagination = new PageInfoModel(_pageSize, totalPages, page)
        });
    }

    public async Task<DishDto> GetDish(Dish dish)
    {
        await _contexts.Ratings.Where(x => x.Dish.Id == dish.Id).ToListAsync();
        return  _mapper.Map<DishDto>(dish);
    }

    public Task<bool> CheckRating(User user, Dish dish)
    {
       return  Task.FromResult(_contexts.Orders.Where(x => x.User.Id == user.Id)
            .Where(x=>x.Status == OrderStatus.Delivered)
            .Any(ord => ord.DishInCart.Any(cart => cart.Dish.Id == dish.Id)));
    }

    public async Task SetRating(User user, Dish dish, int ratingScore)
    {
        Helpers.CheckInfoHelper.CheckRating(ratingScore);
        var ratingEntity = await _contexts.Ratings.SingleOrDefaultAsync(x => x.User == user && x.Dish == dish);
        if (ratingEntity != null)
        {
            ratingEntity.Value = ratingScore;
            _contexts.Ratings.Update(ratingEntity);
            dish.Ratings.FirstOrDefault(x => x.Id == ratingEntity.Id).Value = ratingScore;
            user.Ratings.FirstOrDefault(x => x.Id == ratingEntity.Id).Value = ratingScore;
        }
        else
        {
            var rating = new Rating
            {
                Id = new Guid(),
                Dish = dish,
                User = user,
                Value = ratingScore
            };
            dish.Ratings ??= new List<Rating>();
            dish.Ratings.Add(rating);
            user.Ratings ??= new List<Rating>();
            user.Ratings.Add(rating);
        }

        dish.Rating = dish.Ratings.Sum(x => x.Value) / dish.Ratings.Count;
        await _contexts.SaveChangesAsync();
    }

    private static IQueryable<Dish> GetVegetarian(IQueryable<Dish> dishes, bool vegetarian = false)
    {
        return vegetarian ? dishes.Where(x => x.Vegetarian == vegetarian) : dishes;
    }

    private static IQueryable<Dish> GetCategory(IQueryable<Dish> dishes, DishCategory[]? category = null)
    {
        if (category != null && category.Any())
        {
            return dishes.Where(x => category.Contains(x.Category));
        }

        return dishes;
    }

    private static IQueryable<Dish> SortDishes(IQueryable<Dish> dishes, DishSorting? dishSortings = null)
    {
        return dishSortings switch
        {
            NameAsc => dishes.OrderBy(x => x.Name),
            NameDesc => dishes.OrderByDescending(x => x.Name),
            PriceAsc => dishes.OrderBy(x => x.Price),
            PriceDesc => dishes.OrderByDescending(x => x.Price),
            RatingAsc => dishes.OrderBy(x => x.Rating),
            RatingDesc => dishes.OrderByDescending(x => x.Rating),
            null => dishes,
            _ => throw new ArgumentOutOfRangeException(nameof(dishSortings), dishSortings, null)
        };
    }
}