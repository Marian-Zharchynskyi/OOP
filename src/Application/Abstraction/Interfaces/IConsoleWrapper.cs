namespace Application.Abstraction.Interfaces;

public interface IConsoleWrapper
{
    void WriteLine(string message);
    string ReadLine();
}