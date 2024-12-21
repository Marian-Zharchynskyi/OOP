using Application.Abstraction.Interfaces;
using Infrastructure.Loggers;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.LoggersFactory;

public class LoggerFactory
{
    private readonly IConfiguration _configuration;
    private readonly IConsoleWrapper _consoleWrapper;

    public LoggerFactory(IConfiguration configuration, IConsoleWrapper consoleWrapper)
    {
        _configuration = configuration;
        _consoleWrapper = consoleWrapper;
    }

    public ILogger CreateLogger()
    {
        var loggerType = _configuration["LoggerSettings:LoggerType"];
        return loggerType switch
        {
            "Console" => new ConsoleLogger(_consoleWrapper),
            "File" => new FileLogger(GetFilePath(), _consoleWrapper),
            _ => throw new InvalidOperationException($"Unsupported logger type: {loggerType}")
        };
    }

    private string GetFilePath()
    {
        var filePath = _configuration["LoggerSettings:FilePath"];
        if (string.IsNullOrEmpty(filePath))
        {
            throw new InvalidOperationException("FilePath must be specified for FileLogger in appsettings.json.");
        }
        return filePath;
    }
}