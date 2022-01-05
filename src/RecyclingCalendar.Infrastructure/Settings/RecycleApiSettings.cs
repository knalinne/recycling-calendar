using System.ComponentModel.DataAnnotations;

namespace RecyclingCalendar.Infrastructure.Settings;

public class RecyclingApiSettings
{
    
    public const string ConfigurationRootName = "RecyclingApi";
    [Required] public string Secret { get; set; } = null!;
    [Required] public string Consumer { get; set; } = null!;
}