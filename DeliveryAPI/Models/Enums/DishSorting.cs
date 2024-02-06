using System.Text.Json.Serialization;

namespace DeliveryAPI.Models.Enums;
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DishSorting
{
   NameAsc,
   NameDesc,
   PriceAsc,
   PriceDesc,
   RatingAsc,
   RatingDesc
}