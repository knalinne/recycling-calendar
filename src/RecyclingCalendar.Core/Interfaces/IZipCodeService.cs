using RecyclingCalendar.Core.DTO;

namespace RecyclingCalendar.Core.Interfaces;

public interface IZipCodeService
{
    Task<IList<ZipCode>> FindAll();
    Task<IList<ZipCode>> FindByCode(string zipCode);
}