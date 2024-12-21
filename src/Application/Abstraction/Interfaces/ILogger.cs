namespace Application.Abstraction.Interfaces;

public interface ILogger
{
    void Log(string message);
    void LogError(Exception ex, string message);
}