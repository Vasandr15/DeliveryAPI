using DeliveryAPI.DTO;
using DeliveryAPI.Models;
using DeliveryAPI.Services.AddressService;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryAPI.Controllers;

[ApiController]
[Route("api/address")]
public class AddressController : Controller
{
    private readonly IAddressService _addressService;

    public AddressController(IAddressService addressService)
    {
        _addressService = addressService;
    }

    [HttpGet]
    [Route("search")]
    public async Task<List<SearchAddressModel>?> SearchAddress(int parentObjectId, string? query = null)
    {
        return await _addressService.SearchAddress(parentObjectId, query);
    }

    [HttpGet]
    [Route("chain")]
    public async Task<List<SearchAddressModel>?> GetAddressChain(Guid objectGuid)
    {
        return await _addressService.GetAddressChain(objectGuid);
    }
}