using Application.Abstraction.Interfaces;

namespace Infrastructure.ConsoleWrappers;

public class ConsoleWrapper : IConsoleWrapper
{
    public void WriteLine(string message)
    {
        Console.WriteLine(message);
    }

    public string ReadLine()
    {
        return Console.ReadLine() ?? string.Empty;
    }
}