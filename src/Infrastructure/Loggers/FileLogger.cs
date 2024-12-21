using Application.Abstraction.Interfaces;
using System.IO;

namespace Infrastructure.Loggers
{
    public class FileLogger : ILogger
    {
        private readonly string _filePath;
        private readonly IConsoleWrapper _consoleWrapper;

        public FileLogger(string filePath, IConsoleWrapper consoleWrapper)
        {
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            _consoleWrapper = consoleWrapper;

            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        public void Log(string message)
        {
            WriteToFile($"INFO: {message}");
        }

        public void LogError(Exception ex, string message)
        {
            WriteToFile($"ERROR: {message}");
            WriteToFile($"EXCEPTION: {ex.Message}");
        }

        private void WriteToFile(string logMessage)
        {
            try
            {
                using (var writer = new StreamWriter(_filePath, true))
                {
                    writer.WriteLine($"{DateTime.Now}: {logMessage}");
                }
            }
            catch (Exception ex)
            {
                _consoleWrapper.WriteLine($"Failed to write to file: {ex.Message}");
            }
        }
    }
}