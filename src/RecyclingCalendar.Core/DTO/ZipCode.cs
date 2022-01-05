namespace RecyclingCalendar.Core.DTO;

public class ZipCode
{
    public ZipCode(string code, string id)
    {
        Code = code;
        Id = id;
    }

    public string Code { get; set; }
    public string Id { get; set; }
}