using Application.Abstraction.Interfaces;
using FluentAssertions;
using Infrastructure.Loggers;
using Infrastructure.LoggersFactory;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Xunit;

namespace Api.Tests.Integration;

public class LoggerTests
{
    [Fact]
    public void ConsoleLogger_ShouldWriteInfoMessageToConsole()
    {
        // Arrange
        var mockConsoleWrapper = Substitute.For<IConsoleWrapper>();
        var logger = new ConsoleLogger(mockConsoleWrapper);
        string message = "Test Info Message";

        // Act
        logger.Log(message);

        // Assert
        mockConsoleWrapper.Received(1).WriteLine(Arg.Is<string>(s => s.Contains("INFO: Test Info Message")));
    }

    [Fact]
    public void ConsoleLogger_ShouldWriteErrorMessageAndExceptionToConsole()
    {
        // Arrange
        var mockConsoleWrapper = Substitute.For<IConsoleWrapper>();
        var logger = new ConsoleLogger(mockConsoleWrapper);
        var exception = new Exception("Test Exception");
        string message = "Test Error Message";

        // Act
        logger.LogError(exception, message);

        // Assert
        mockConsoleWrapper.Received(1).WriteLine(Arg.Is<string>(s => s.Contains("ERROR: Test Error Message")));
        mockConsoleWrapper.Received(1).WriteLine(Arg.Is<string>(s => s.Contains("EXCEPTION: Test Exception")));
    }

    [Fact]
    public void FileLogger_ShouldWriteMessageToFile()
    {
        // Arrange
        string filePath = Path.Combine(Path.GetTempPath(), "log.txt");
        var mockConsoleWrapper = Substitute.For<IConsoleWrapper>();
        var logger = new FileLogger(filePath, mockConsoleWrapper);
        string message = "Test Info Message";

        // Act
        logger.Log(message);

        // Assert
        var fileContent = File.ReadAllText(filePath);
        fileContent.Should().Contain("INFO: Test Info Message");

        File.Delete(filePath);
    }

    [Fact]
    public void FileLogger_ShouldWriteErrorMessageAndExceptionToFile()
    {
        // Arrange
        string filePath = Path.Combine(Path.GetTempPath(), "logError.txt");
        var mockConsoleWrapper = Substitute.For<IConsoleWrapper>();
        var logger = new FileLogger(filePath, mockConsoleWrapper);
        var exception = new Exception("Test Exception");
        string message = "Test Error Message";

        // Act
        logger.LogError(exception, message);

        // Assert
        var fileContent = File.ReadAllText(filePath);
        fileContent.Should().Contain("ERROR: Test Error Message");
        fileContent.Should().Contain("EXCEPTION: Test Exception");

        // Clean up
        File.Delete(filePath);
    }

    [Fact]
    public void LoggerFactory_ShouldReturnConsoleLogger_WhenConfiguredForConsole()
    {
        // Arrange
        var mockConsoleWrapper = Substitute.For<IConsoleWrapper>();
        var config = Substitute.For<IConfiguration>();
        config["LoggerSettings:LoggerType"].Returns("Console");

        var factory = new LoggerFactory(config, mockConsoleWrapper);

        // Act
        var logger = factory.CreateLogger();

        // Assert
        logger.Should().BeOfType<ConsoleLogger>();
    }

    [Fact]
    public void LoggerFactory_ShouldReturnFileLogger_WhenConfiguredForFile()
    {
        // Arrange
        var mockConsoleWrapper = Substitute.For<IConsoleWrapper>();
        var config = Substitute.For<IConfiguration>();

        string tempFilePath = Path.Combine(Path.GetTempPath(), "log.txt");

        config["LoggerSettings:LoggerType"].Returns("File");
        config["LoggerSettings:FilePath"].Returns(tempFilePath);

        var factory = new LoggerFactory(config, mockConsoleWrapper);

        // Act
        var logger = factory.CreateLogger();

        // Assert
        logger.Should().BeOfType<FileLogger>();

        
        if (File.Exists(tempFilePath))
        {
            File.Delete(tempFilePath);
        }
    }


    [Fact]
    public void LoggerFactory_ShouldThrowException_WhenInvalidLoggerType()
    {
        // Arrange
        var mockConsoleWrapper = Substitute.For<IConsoleWrapper>();
        var config = Substitute.For<IConfiguration>();
        config["LoggerSettings:LoggerType"].Returns("InvalidType");

        var factory = new LoggerFactory(config, mockConsoleWrapper);

        // Act & Assert
        Action act = () => factory.CreateLogger();
        act.Should().Throw<InvalidOperationException>().WithMessage("Unsupported logger type: InvalidType");
    }
}