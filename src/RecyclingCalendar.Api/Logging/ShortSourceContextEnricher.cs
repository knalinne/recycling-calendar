using Serilog.Core;
using Serilog.Events;

namespace RecyclingCalendar.Api.Logging;

public class ShortSourceContextEnricher : ILogEventEnricher
{
    public ShortSourceContextEnricher()
    {
    }

    public ShortSourceContextEnricher(string propertyName, string? ansiColor)
    {
        PropertyName = propertyName;
        AnsiColor = ansiColor;
    }

    public int MaxLength { get; set; } = 40;

    public string PropertyName { get; set; } = "SourceContext";

    public string? AnsiColor { get; set; } = null;

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var sourceContext = logEvent.Properties.GetValueOrDefault("SourceContext");
        if (sourceContext == null) return;
        var split = sourceContext.ToString().Trim('"').Split(".");
        for (var i = 0; i < split.Length - 1; i++)
        {
            if (string.Join(".", split).Length <= MaxLength)
                break;
            split[i] = split[i][0].ToString();
        }

        logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty(PropertyName,
            (AnsiColor ?? "") + string.Join(".", split)));
    }
}