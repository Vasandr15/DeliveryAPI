using System.Text.RegularExpressions;
using AutoMapper;
using AutoMapper.Internal;
using DeliveryAPI.DbContext;
using DeliveryAPI.Exceptions;
using DeliveryAPI.Models;

namespace DeliveryAPI.Services.AddressService;

public class AddressService : IAddressService
{
    private readonly DeliveryContext _context;
    private readonly IMapper _mapper;

    public AddressService(DeliveryContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<SearchAddressModel>?> SearchAddress(int parentObjectId = 0, string? query = null)
    {

        var hierarchyObjectId = _context.AsAdmHierarchies
            .Where(x => x.Parentobjid == parentObjectId)
            .Select(x => x.Objectid).ToList();
        
            var regex = query == null ? new Regex(".") : new Regex(query.ToLower());

            return await Task.FromResult((List<SearchAddressModel>?)SearchHouses(hierarchyObjectId, regex)
                .Concat(SearchAddrObj(hierarchyObjectId, regex)).Take(10).ToList());
    }

    public Task<List<SearchAddressModel>?> GetAddressChain(Guid objectGuid)
    {
        var house =  _context.AsHouses.FirstOrDefault(x => x.Objectguid == objectGuid);
        var obj = _context.AsAddrObjs.FirstOrDefault(x => x.Objectguid == objectGuid);

        if (house != null) return Task.FromResult(GetHouseChain(house.Objectid));
        if (obj != null) return Task.FromResult(GetObjectChain(obj.Objectid));
        throw new NotFoundException($"Not found object with id={objectGuid}");
    }

    private List<SearchAddressModel>? GetObjectChain(long objectId)
    {
        var path = _context.AsAdmHierarchies
            .Where(x => x.Objectid == objectId)
            .Select(x => x.Path)
            .FirstOrDefault();

        if (string.IsNullOrEmpty(path)) return null;
        {
            var pathComponents = path.Split('.').ToList();

            var results = pathComponents.Select(pathComponent =>
                _context.AsAddrObjs.FirstOrDefault(x => x.Objectid.ToString() == pathComponent));

           return results.Select(item => _mapper.Map<SearchAddressModel>(item)).ToList();
        }

    }

    private List<SearchAddressModel>? GetHouseChain(long objectId)
    {
        var path = _context.AsAdmHierarchies
            .Where(x => x.Objectid == objectId)
            .Select(x => x.Path)
            .FirstOrDefault();

        if (string.IsNullOrEmpty(path)) return null;
        {
            var pathComponents = path.Split('.').ToList();
            pathComponents.Remove(objectId.ToString());

            var results = pathComponents.Select(pathComponent => 
                    _context.AsAddrObjs.FirstOrDefault(x => x.Objectid.ToString() == pathComponent))
                .Where(addrObj => addrObj != null).ToList();

            var house = _context.AsHouses
                .FirstOrDefault(x => x.Objectid == objectId);

            var searchAddressModels = results.Select(item => _mapper.Map<SearchAddressModel>(item)).ToList();
            searchAddressModels.Add(_mapper.Map<SearchAddressModel>(house));
            
            return searchAddressModels;
        }
    }

    private List<SearchAddressModel> SearchHouses(List<long?> hierarchyObjectId, Regex regex)
    {
        var houses = _context.AsHouses.OrderByDescending(x => x.Isactive)
            .ThenByDescending(x => x.Isactual)
            .Where(x=>x.Isactual == 1)
            .Where(x => hierarchyObjectId.Contains(x.Objectid)).AsEnumerable()
            .Where(x => x.Housenum != null && regex.Match(x.Housenum).Success).ToList();

        return houses.Select(house => _mapper.Map<SearchAddressModel>(house)).ToList();
    }

    private List<SearchAddressModel> SearchAddrObj(List<long?> hierarchyObjectId, Regex regex)
    {
        var addressObj = _context.AsAddrObjs.OrderByDescending(x => x.Isactive)
            .ThenByDescending(x => x.Isactual)
            .Where(x=>x.Isactual == 1)
            .Where(x => hierarchyObjectId.Contains(x.Objectid)).AsEnumerable()
            .Where(x => regex.Match(x.Name.ToLower()).Success).ToList();

        return addressObj.Select(obj => _mapper.Map<SearchAddressModel>(obj)).ToList();
    }
}