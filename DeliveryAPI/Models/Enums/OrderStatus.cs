using System.Text.Json.Serialization;

namespace DeliveryAPI.Models.Enums;
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OrderStatus
{
    InProcess,
    Delivered
}