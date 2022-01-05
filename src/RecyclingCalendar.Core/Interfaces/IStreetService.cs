using RecyclingCalendar.Core.DTO;

namespace RecyclingCalendar.Core.Interfaces;

public interface IStreetService
{
    Task<IList<Street>> FindAll(string zipCodeId);
    Task<IList<Street>> FindByName(string zipCodeId, string name);
}