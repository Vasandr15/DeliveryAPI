using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DeliveryAPI.Configurations;
using DeliveryAPI.Exceptions;
using DeliveryAPI.Helpers;
using DeliveryAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace DeliveryAPI.Services.JwtService;

public class JwtService : IJwtService
{
    private readonly ApplicationDbContexts _contexts;

    public JwtService(ApplicationDbContexts contexts)
    {
        _contexts = contexts;
    }

    public async Task<ClaimsIdentity> GetIdentity(LoginCredentials credentials)
    {
        var user = await _contexts.Users.FirstOrDefaultAsync(x =>
            x.Email == credentials.Email
        );

        if (user == null)
        {
            throw new BadRequest("Login failed");
        }

        if (!HashPasswordHelper.VerifyPassword(credentials.Password, user.Password))
        {
            throw new BadRequest("Wrong password");
        }
        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Hash, Guid.NewGuid().ToString())
        };

        var claimsIdentity =
            new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
        
        return claimsIdentity;
    }

    public string GetToken(ClaimsIdentity identity)
    {
        var now = DateTime.UtcNow;

        var jwt = new JwtSecurityToken(
            issuer: JwtConfigurations.Issuer,
            audience: JwtConfigurations.Audience,
            notBefore: now,
            claims: identity?.Claims,
            expires: now.AddMinutes(JwtConfigurations.LifeTime),
            signingCredentials: new SigningCredentials(JwtConfigurations.GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
    
}