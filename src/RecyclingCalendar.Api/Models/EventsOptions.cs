using System.ComponentModel.DataAnnotations;

namespace RecyclingCalendar.Api.Models;

public class EventsOptions
{
    [RegularExpression("([01]?[0-9]|2[0-3]):[0-5][0-9]", ErrorMessage = "Not a valid time, must be hh:mm")] public string? AlarmTime { get; set; }
}