using System.ComponentModel.DataAnnotations;
namespace DeliveryAPI.DTO;

public class TokenResponse
{
    public TokenResponse(string token)
    {
        Token = token;
    }

    [Required]
    [MinLength(1)]
    public string Token { get; set; }
}