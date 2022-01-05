using Microsoft.Extensions.Caching.Memory;
using RecyclingCalendar.Core.DTO;
using RecyclingCalendar.Core.Interfaces;

namespace RecyclingCalendar.Core.Services;

public class RecyclingEventService:IRecyclingEventService
{
    private readonly IMemoryCache _cache;
    private readonly IRecyclingApiClient _recyclingApiClient;

    public RecyclingEventService(IMemoryCache cache, IRecyclingApiClient recyclingApiClient)
    {
        _cache = cache;
        _recyclingApiClient = recyclingApiClient;
    }

    public async Task<IList<RecyclingEvent>> FindBy(string zipCodeId, string streetId, int houseNumber)
    {
        if (_cache.TryGetValue(CacheKeys.RecyclingEventsBy(zipCodeId, streetId, houseNumber), out IList<RecyclingEvent> recyclingEvents)) return recyclingEvents;
        
        recyclingEvents = await _recyclingApiClient.FindRecyclingEventsBy(zipCodeId, streetId, houseNumber);
        _cache.Set(CacheKeys.RecyclingEventsBy(zipCodeId, streetId, houseNumber), recyclingEvents,
            new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromDays(1)));

        return recyclingEvents;
    }
}