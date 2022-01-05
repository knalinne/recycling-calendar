using Microsoft.Extensions.Caching.Memory;
using RecyclingCalendar.Core.DTO;
using RecyclingCalendar.Core.Interfaces;

namespace RecyclingCalendar.Core.Services;

public class StreetService: IStreetService
{
    private readonly IRecyclingApiClient _recyclingApiClient;
    private readonly IMemoryCache _cache;

    public StreetService(IRecyclingApiClient recyclingApiClient, IMemoryCache cache)
    {
        _recyclingApiClient = recyclingApiClient;
        _cache = cache;
    }

    public async Task<IList<Street>> FindAll(string zipCodeId)
    {
        if (_cache.TryGetValue(CacheKeys.AllStreetByZipCodeId(zipCodeId), out IList<Street> streets)) return streets;
        
        streets = await _recyclingApiClient.FindAllStreets(zipCodeId);
        _cache.Set(CacheKeys.AllStreetByZipCodeId(zipCodeId), streets,
            new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromDays(1)));

        return streets;
    }

    public async Task<IList<Street>> FindByName(string zipCodeId, string name)
    {
        if (_cache.TryGetValue(CacheKeys.AllStreetByZipCodeIdAndName(zipCodeId, name), out IList<Street> streets)) return streets;
        
        streets = await _recyclingApiClient.FindStreetsByName(zipCodeId, name);
        _cache.Set(CacheKeys.AllStreetByZipCodeIdAndName(zipCodeId, name), streets,
            new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromDays(1)));

        return streets;
    }
}