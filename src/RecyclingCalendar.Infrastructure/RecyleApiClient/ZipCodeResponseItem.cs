namespace RecyclingCalendar.Infrastructure.RecyleApiClient;

public class ZipCodeResponseItem
{
    public string Code { get; set; }
    public string Id { get; set; }
    public IList<NamesResponse> Names { get; set; }
    
}