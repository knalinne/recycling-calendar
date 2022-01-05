using Serilog;
using Serilog.Configuration;

namespace RecyclingCalendar.Api.Logging;

public static class SerilogExtensions
{
    public static LoggerConfiguration WithShortSourceContext(this LoggerEnrichmentConfiguration enrich)
    {
        if (enrich == null)
            throw new ArgumentNullException(nameof(enrich));
        return enrich.With<ShortSourceContextEnricher>();
    }

    public static LoggerConfiguration WithColoredShortSourceContext(this LoggerEnrichmentConfiguration enrich)
    {
        if (enrich == null)
            throw new ArgumentNullException(nameof(enrich));
        return enrich.With(new ShortSourceContextEnricher("ColoredSourceContext",
            SerilogConfig.DefaultContextAnsiColor));
    }
}