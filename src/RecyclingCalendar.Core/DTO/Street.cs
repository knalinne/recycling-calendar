namespace RecyclingCalendar.Core.DTO;

public class Street
{
    public Street(string name, string id)
    {
        Name = name;
        Id = id;
    }

    public string Name { get; set; }
    public string Id { get; set; }
}