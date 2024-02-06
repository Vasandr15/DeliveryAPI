using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using DeliveryAPI.DbContext;
using DeliveryAPI.DTO;
using DeliveryAPI.Exceptions;
using DeliveryAPI.Helpers;
using DeliveryAPI.Models;
using DeliveryAPI.Models.DbEntities;
using DeliveryAPI.Models.DTO;
using DeliveryAPI.Services.JwtService;
using Microsoft.EntityFrameworkCore;

namespace DeliveryAPI.Services.UserServices;

public class UserServices : IUserServices
{
    private readonly ApplicationDbContexts _contexts;
    private readonly DeliveryContext _delivery;
    private readonly IMapper _mapper;
    private readonly IJwtService _jwt;

    public UserServices(ApplicationDbContexts contexts, IMapper mapper, IJwtService jwt, DeliveryContext delivery)
    {
        _contexts = contexts;
        _mapper = mapper;
        _jwt = jwt;
        _delivery = delivery;
    }

    public async Task<TokenResponse> RegisterUser(UserRegisterModel userRegisterModel)
    {
        
        await CheckInfoHelper.CheckAddress(userRegisterModel.AddressId, _delivery);
        CheckInfoHelper.CheckGender(userRegisterModel.Gender);
        CheckInfoHelper.CheckValidDate(userRegisterModel.BirthDate);
        CheckInfoHelper.CheckPhone(userRegisterModel.PhoneNumber);
        await CheckUniqueEmail(CheckInfoHelper.NormilizeEmail(userRegisterModel.Email));

        var credentials = new LoginCredentials
        {
            Email = userRegisterModel.Email,
            Password = userRegisterModel.Password,
        };
        
        userRegisterModel.Password = HashPasswordHelper.HashPassword(userRegisterModel.Password);
        await _contexts.Users.AddAsync(_mapper.Map<User>(userRegisterModel));
        await _contexts.SaveChangesAsync();

        return await LogInUser(credentials);
    }

    public async Task<TokenResponse> LogInUser(LoginCredentials loginCredentials)
    {
        var identity = await _jwt.GetIdentity(loginCredentials);
        if (identity == null)
        {
            throw new Exception("Login failed");
        }
        var token = _jwt.GetToken(identity);
        
        return new TokenResponse(token);
    }

    public UserDto GetUserProfile(User user)
    {
        return  _mapper.Map<UserDto>(user);
    }

    public async Task<Response> LogoutUser(HttpContext httpContext)
    {
        var tokenString = httpContext.Request.Headers["Authorization"].First()?.Replace("Bearer ", "");
        var token = new Token()
        {
            Id = Guid.NewGuid(),
            ValidToken = tokenString,
            ExpiredDate = new JwtSecurityTokenHandler().ReadJwtToken(tokenString).ValidTo
        };

        await _contexts.Tokens.AddRangeAsync(token);
        await _contexts.SaveChangesAsync();
        return new Response() 
        {
            Status = "OK",
            Message = "You've been successfully logged out"
        };
    }

    public async Task EditProfile(UserEditModel userEditModel, User user)
    {
        CheckInfoHelper.CheckGender(userEditModel.Gender);
        CheckInfoHelper.CheckValidDate(userEditModel.BirthDate);
        CheckInfoHelper.CheckPhone(userEditModel.PhoneNumber); 
        await CheckInfoHelper.CheckAddress(userEditModel.AddressId, _delivery);

        user.FullName = userEditModel.FullName;
        user.PhoneNumber = userEditModel.PhoneNumber;
        user.BirthDate = userEditModel.BirthDate;
        user.Gender = userEditModel.Gender;
        user.AddressId = userEditModel.AddressId;
        
        await _contexts.SaveChangesAsync();
    }
    
    private async Task CheckUniqueEmail(string email)
    {
        var dbEmail = await _contexts
            .Users
            .Where(x => email == x.Email)
            .FirstOrDefaultAsync();

        if (dbEmail == null) return;
        throw new BadRequest($"Username {email} is already taken.");
    }
}