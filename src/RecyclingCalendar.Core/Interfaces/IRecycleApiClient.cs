using RecyclingCalendar.Core.DTO;

namespace RecyclingCalendar.Core.Interfaces;

public interface IRecyclingApiClient
{
    Task<IList<ZipCode>> FindAllZipCodes();
    Task<IList<ZipCode>> FindZipCodesByCode(string? zipCode);
    Task<IList<Street>> FindAllStreets(string zipCodeId);
    Task<IList<Street>> FindStreetsByName(string zipCodeId, string? name);
    Task<IList<RecyclingEvent>> FindRecyclingEventsBy(string zipCodeId, string streetId, int houseNumber);
}