using DeliveryAPI.DTO;
using DeliveryAPI.Models;
using DeliveryAPI.Models.DTO;
using DeliveryAPI.Services.Repository;
using DeliveryAPI.Services.UserServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryAPI.Controllers;

[ApiController]
[Route("api/account")]
public class UserController : Controller
{
    private readonly IUserServices _userServices;
    private readonly IRepositoryService _repositoryService;

    public UserController(IUserServices userServices, IRepositoryService repositoryService)
    {
        _userServices = userServices;
        _repositoryService = repositoryService;
    }

    [HttpPost]
    [Route("register")]
    public async Task<TokenResponse> RegisterUser(UserRegisterModel userRegisterModel)
    {
        return await _userServices.RegisterUser(userRegisterModel);
    }

    [HttpPost]
    [Route("login")]
    public async Task<TokenResponse> LoginUser(LoginCredentials loginCredentials)
    {
        return await _userServices.LogInUser(loginCredentials);
    }

    [HttpGet]
    [Authorize]
    [Authorize(Policy = "AuthorizationPolicy")]
    [Route("profile")]
    public async Task<UserDto> GetUserProfile ()
    {
        return  _userServices.GetUserProfile(await _repositoryService.GetUser(User));
    }

    [HttpPost]
    [Authorize]
    [Authorize(Policy = "AuthorizationPolicy")]
    [Route("logout")]
    public async Task<Response> LogoutUser()
    {
       return await _userServices.LogoutUser(HttpContext);
    }

    [HttpPut]
    [Authorize(Policy = "AuthorizationPolicy")]
    [Route("profile")]
    public async Task<Response> EditProfile(UserEditModel userEditModel)
    {
        await _userServices.EditProfile(userEditModel, await _repositoryService.GetUser(User));
        return new Response()
        {
            Status = "Ok",
            Message = "You have successfully edited your profile"
        };
    }
}