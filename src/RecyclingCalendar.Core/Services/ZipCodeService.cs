using Microsoft.Extensions.Caching.Memory;
using RecyclingCalendar.Core.DTO;
using RecyclingCalendar.Core.Interfaces;

namespace RecyclingCalendar.Core.Services;

public class ZipCodeService: IZipCodeService
{
    private readonly IRecyclingApiClient _recyclingApiClient;
    private readonly IMemoryCache _cache;

    public ZipCodeService(IRecyclingApiClient recyclingApiClient, IMemoryCache cache)
    {
        _recyclingApiClient = recyclingApiClient;
        _cache = cache;
    }

    public async Task<IList<ZipCode>> FindAll()
    {
        if (_cache.TryGetValue(CacheKeys.AllZipCode, out IList<ZipCode> zipCodes)) return zipCodes;
        
        zipCodes = await _recyclingApiClient.FindAllZipCodes();
        _cache.Set(CacheKeys.AllZipCode, zipCodes,
            new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromDays(1)));

        return zipCodes;
    }

    public async Task<IList<ZipCode>> FindByCode(string zipCode)
    {
        if (_cache.TryGetValue(CacheKeys.ZipCodeByCode(zipCode), out IList<ZipCode> zipCodes)) return zipCodes;
        
        zipCodes = await _recyclingApiClient.FindZipCodesByCode(zipCode);
        _cache.Set(CacheKeys.ZipCodeByCode(zipCode), zipCodes,
            new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromDays(1)));

        return zipCodes;
    }
}