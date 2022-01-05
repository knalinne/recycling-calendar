namespace RecyclingCalendar.Infrastructure.RecyleApiClient;

public class RecyclingEventResponseItem
{
    public DateTime Timestamp { get; set; }
    public RecyclingEventFractionResponseItem Fraction { get; set; }
    
}

public class RecyclingEventFractionResponseItem
{
    public NamesResponse Name { get; set; }
}