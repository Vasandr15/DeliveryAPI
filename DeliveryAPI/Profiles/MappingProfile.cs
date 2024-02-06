using AutoMapper;
using DeliveryAPI.Helpers;
using DeliveryAPI.Models;
using DeliveryAPI.Models.Address;
using DeliveryAPI.Models.DbEntities;
using DeliveryAPI.Models.DTO;
using DeliveryAPI.Models.Enums;

namespace DeliveryAPI.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<UserRegisterModel, User>();
        CreateMap<User, UserDto>();
        CreateMap<UserEditModel, User>();
        CreateMap<Dish, DishDto>();
        CreateMap<DishInCart, DishBasketDto>()
            .ForMember(x=>x.Id, o
                => o.MapFrom(dishBasket => dishBasket.Dish.Id))
            .ForMember(x => x.Name, o
                => o.MapFrom(dishBasket => dishBasket.Dish.Name))
            .ForMember(x => x.Image, o
                => o.MapFrom(dishBasket => dishBasket.Dish.Image))
            .ForMember(x => x.Price, o
                => o.MapFrom(dishBasket => dishBasket.Dish.Price))
            .ForMember(x => x.TotalPrice, o
                => o.MapFrom(dishBasket => dishBasket.Dish.Price * dishBasket.Amount));

        CreateMap<Order, OrderDto>();

        CreateMap<AsAddrObj, SearchAddressModel>()
            .ForMember(x => x.Text, o
                => o.MapFrom(x => x.Typename+ " " + x.Name))
            .ForMember(x => x.ObjectLevel, opt =>
            {
                GarAddressLevel addressLevel;
                opt.MapFrom(src =>
                        Enum.TryParse(src.Level, out addressLevel) ? (int)addressLevel : 0);
            })
            .ForMember(x => x.ObjectLevelText, o
                => o.MapFrom(x => ConstantStrings.garLevel[int.Parse(x.Level)]));

        CreateMap<AsHouse, SearchAddressModel>()
            .ForMember(x => x.Text, o
                => o.MapFrom(x => x.Housenum )) 
            .ForMember(x=>x.ObjectLevel, o
                =>o.MapFrom(_=>"Building"))
            .ForMember(x => x.ObjectLevelText, o
                => o.MapFrom(_ => ConstantStrings.garLevel[9])); 
        
    }
}