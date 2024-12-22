using Application.Abstraction.Interfaces;

namespace Application.Implementation
{
    public class OrderNotifier : INotifier
    {
        private readonly List<IObserver> _observers = new();
        private readonly ILogger _logger; 

        public OrderNotifier(ILogger logger)
        {
            _logger = logger;
        }

        public void Attach(IObserver observer)
        {
            _observers.Add(observer);
            _logger.Log("Observer attached."); 
        }

        public void Detach(IObserver observer)
        {
            _observers.Remove(observer);
            _logger.Log("Observer detached."); 
        }

        public void Notify(string message)
        {
            foreach (var observer in _observers)
            {
                observer.Update(message);
            }
            _logger.Log($"Notification sent: {message}"); 
        }
    }
}