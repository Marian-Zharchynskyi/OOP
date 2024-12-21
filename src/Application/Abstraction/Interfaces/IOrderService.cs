using Domain.Orders;
using Domain.Products;

namespace Application.Abstraction.Interfaces;

public interface IOrderService
{
    Task<IReadOnlyList<Order>> GetAllOrdersAsync();
    Task<Order> GetOrderByIdAsync(Guid id);
    Task<Order> CreateOrderAsync(List<Product> products);
    Task<Order> UpdateOrderAsync(Order order);
    Task<Order> DeleteOrderAsync(Guid id);
}