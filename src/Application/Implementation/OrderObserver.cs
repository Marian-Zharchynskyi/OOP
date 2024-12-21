using Application.Abstraction.Interfaces;
using Domain.Orders;

namespace Application.Implementation;

public class OrderObserver : IObserver
{
    private readonly string _observerName;

    public OrderObserver(string observerName)
    {
        _observerName = observerName;
    }

    public void Update(Order order)
    {
        Console.WriteLine(
            $"[{_observerName}] Замовлення з ID {order.Id} було оновлено. Загальна сума: {order.TotalAmount}");
    }
}