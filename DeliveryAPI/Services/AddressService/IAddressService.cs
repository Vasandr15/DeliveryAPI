using DeliveryAPI.Models;

namespace DeliveryAPI.Services.AddressService;

public interface IAddressService
{
    public Task<List<SearchAddressModel>?> SearchAddress(int parentObjectId, string? query);
    public Task<List<SearchAddressModel>?> GetAddressChain(Guid objectGuid);
}