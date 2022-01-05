using RecyclingCalendar.Core.DTO;

namespace RecyclingCalendar.Core.Interfaces;

public interface IRecyclingEventService
{
    Task<IList<RecyclingEvent>> FindBy(string zipCodeId, string streetId, int houseNumber);
}