using Serilog.Sinks.SystemConsole.Themes;

namespace RecyclingCalendar.Api.Logging;

public class SerilogConfig
{
    public static readonly AnsiConsoleTheme DefaultColorTheme = new(
        new Dictionary<ConsoleThemeStyle, string>
        {
            [ConsoleThemeStyle.Text] = "\x1b[38;5;0253m",
            [ConsoleThemeStyle.SecondaryText] = "\x1b[38;5;0246m",
            [ConsoleThemeStyle.TertiaryText] = "\x1b[38;5;0242m",
            [ConsoleThemeStyle.Invalid] = "\x1b[33;1m",
            [ConsoleThemeStyle.Null] = "\x1b[38;5;0038m",
            [ConsoleThemeStyle.Name] = "\x1b[38;5;0081m",
            [ConsoleThemeStyle.String] = "\x1b[38;5;0216m",
            [ConsoleThemeStyle.Number] = "\x1b[38;5;151m",
            [ConsoleThemeStyle.Boolean] = "\x1b[38;5;0038m",
            [ConsoleThemeStyle.Scalar] = "\x1b[38;5;0079m",
            [ConsoleThemeStyle.LevelVerbose] = "\x1b[37m",
            [ConsoleThemeStyle.LevelDebug] = "\x1b[37m",
            [ConsoleThemeStyle.LevelInformation] = "\x1b[38;5;77m",
            [ConsoleThemeStyle.LevelWarning] = "\x1b[38;5;214m",
            [ConsoleThemeStyle.LevelError] = "\x1b[38;5;167m",
            [ConsoleThemeStyle.LevelFatal] = "\x1b[38;5;167m"
        });

    public static readonly string DefaultFileLogTemplate =
        "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] {SourceContext,-40}: {Message:lj}{NewLine}{Exception}";

    public static readonly string DefaultConsoleLogTemplate =
        "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] {ColoredSourceContext,-50}: {Message:lj}{NewLine}{Exception}";

    public static readonly string? DefaultContextAnsiColor = "\x1b[38;5;43m";
}