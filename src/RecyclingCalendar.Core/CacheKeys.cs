namespace RecyclingCalendar.Core;

public static class CacheKeys
{
    public const string AllZipCode = "AllZipCodeCacheKey";

    public static string ZipCodeByCode(string zipCode)
    {
        return $"ZipCodeByCodeCacheKey_{zipCode}";
    }

    public static string AllStreetByZipCodeId(string zipCodeId)
    {
        return $"AllStreetByZipCodeIdCacheKey_{zipCodeId}";
    }

    public static string AllStreetByZipCodeIdAndName(string zipCodeId, string name)
    {
        return  $"AllStreetByZipCodeIdAndNameCacheKey_{zipCodeId}_{name}";
    }

    public static string RecyclingEventsBy(string zipCodeId, string streetId, int houseNumber)
    {
        return  $"RecyclingEventsByZipStreetAndHouse_{zipCodeId}_{streetId}_{houseNumber}";
    }
}