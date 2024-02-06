using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace DeliveryAPI.Configurations;

public abstract class JwtConfigurations
{
    public const string Issuer = "DeliveryBackend";
    public const string Audience = "NoAudience";
    private const string Key = "MosolTheBestMosolTheBestFriend00";
    public const int LifeTime = 60;

    public static SymmetricSecurityKey GetSymmetricSecurityKey()
    {
        return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key));
    }
}