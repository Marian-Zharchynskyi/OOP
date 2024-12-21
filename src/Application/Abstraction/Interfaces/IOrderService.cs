using Domain.Orders;

namespace Application.Abstraction.Interfaces;

public interface IOrderService
{
    Task<IReadOnlyList<Order>> GetAllOrdersAsync();
    Task<Order> GetOrderByIdAsync(Guid id);
    Task<Order> CreateOrderAsync(Order order);
    Task<Order> UpdateOrderAsync(Order order);
    Task<Order> DeleteOrderAsync(Guid id);
}