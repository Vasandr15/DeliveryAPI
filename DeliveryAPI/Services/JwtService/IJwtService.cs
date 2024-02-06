using System.Security.Claims;
using DeliveryAPI.Models;

namespace DeliveryAPI.Services.JwtService;

public interface IJwtService
{
    public string GetToken(ClaimsIdentity claims);
    public Task<ClaimsIdentity> GetIdentity(LoginCredentials credentials);
}