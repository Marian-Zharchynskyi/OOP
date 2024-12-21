using Application.Abstraction.Interfaces;
using Domain.Orders;

namespace Application.Implementation;

using System.Collections.Generic;

public class OrderNotifier
{
    private readonly List<IObserver> _observers = new();

    public void AddObserver(IObserver observer)
    {
        _observers.Add(observer);
    }

    public void RemoveObserver(IObserver observer)
    {
        _observers.Remove(observer);
    }

    public void Notify(Order order)
    {
        foreach (var observer in _observers)
        {
            observer.Update(order);
        }
    }
}

