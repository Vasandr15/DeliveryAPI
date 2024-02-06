using DeliveryAPI.DTO;
using DeliveryAPI.Models;
using DeliveryAPI.Models.DbEntities;
using DeliveryAPI.Models.DTO;

namespace DeliveryAPI.Services.UserServices;

public interface IUserServices
{
    public Task<TokenResponse> RegisterUser(UserRegisterModel userRegisterModel);
    public Task<TokenResponse> LogInUser(LoginCredentials loginCredentials);
    public UserDto GetUserProfile(User user);
    public Task<Response> LogoutUser(HttpContext claimsPrincipal);
    public Task EditProfile(UserEditModel userEditModel, User claimsPrincipal);
}