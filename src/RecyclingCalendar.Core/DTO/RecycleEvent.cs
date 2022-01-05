namespace RecyclingCalendar.Core.DTO;

public class RecyclingEvent
{
    public RecyclingEvent(DateOnly eventDate, string summary = "", string description = "")
    {
        EventDate = eventDate;
        Summary = summary;
        Description = description;
    }

    public DateOnly EventDate { get; set; }
    public string Summary { get; set; }
    public string Description { get; set; }
}