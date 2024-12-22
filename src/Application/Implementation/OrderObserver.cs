using System.Collections;
using Application.Abstraction.Interfaces;

namespace Application.Implementation;

public class OrderObserver : IObserver, IEnumerable<string>
{
    private readonly List<string> _notifications = new();
    private readonly ILogger _logger;

    public OrderObserver(ILogger logger)
    {
        _logger = logger;
    }

    public void Update(string message)
    {
        _notifications.Add(message);
        
        _logger.Log($"Received notification: {message}");
    }

    public IEnumerator<string> GetEnumerator()
    {
        return _notifications.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}