namespace Application.Abstraction.Interfaces;

public interface INotifier
{
    void Attach(IObserver observer);
    void Detach(IObserver observer);
    void Notify(string message);
}
