namespace RecyclingCalendar.Infrastructure.RecyleApiClient;

public class PaginatedResponse<TItem> where TItem: class
{
    public IList<TItem> Items { get; set; }
    public int Total { get; set; }
    public int Pages { get; set; }
    public int Page { get; set; }
    public int Size { get; set; }
}