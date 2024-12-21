using Application.Abstraction.Interfaces;

namespace Infrastructure.Loggers;

public class ConsoleLogger : ILogger
{
    private readonly IConsoleWrapper _consoleWrapper;

    public ConsoleLogger(IConsoleWrapper consoleWrapper)
    {
        _consoleWrapper = consoleWrapper;
    }

    public void Log(string message)
    {
        _consoleWrapper.WriteLine($"INFO: {message}");
    }

    public void LogError(Exception ex, string message)
    {
        _consoleWrapper.WriteLine($"ERROR: {message}");
        _consoleWrapper.WriteLine($"EXCEPTION: {ex.Message}");
    }
}