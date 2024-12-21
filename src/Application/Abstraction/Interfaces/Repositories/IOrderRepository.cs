using Domain.Orders;

namespace Application.Abstraction.Interfaces.Repositories;

public interface IOrderRepository
{
    Task<Order> Add(Order order);
    Task<Order> Update(Order order);
    Task<Order> Delete(Order order);
    Task SaveChangesAsync();
}