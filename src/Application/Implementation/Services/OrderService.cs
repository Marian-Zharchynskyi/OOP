using Application.Abstraction.Interfaces;
using Application.Abstraction.Interfaces.Queries;
using Application.Abstraction.Interfaces.Repositories;
using Domain.Orders;

namespace Application.Implementation.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderQueries _orderQueries;
        private readonly ILogger _logger;

        public OrderService(IOrderRepository orderRepository, IOrderQueries orderQueries, ILogger logger)
        {
            _orderRepository = orderRepository;
            _orderQueries = orderQueries;
            _logger = logger;
        }

        public async Task<IReadOnlyList<Order>> GetAllOrdersAsync()
        {
            try
            {
                var orders = await _orderQueries.GetAll();
                _logger.Log("Fetched all orders successfully.");
                return orders;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all orders.");
                return new List<Order>();
            }
        }

        public async Task<Order> GetOrderByIdAsync(Guid id)
        {
            try
            {
                var order = await _orderQueries.GetById(id);
                if (order == null)
                {
                    _logger.LogError(new KeyNotFoundException($"Order with ID {id} not found."),
                        "Error fetching order by ID.");
                    return null;
                }

                _logger.Log($"Fetched order with ID {id} successfully.");
                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching order by ID.");
                return null;
            }
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            try
            {
                order.UpdateTotalAmount();
                var createdOrder = await _orderRepository.Add(order);
                await _orderRepository.SaveChangesAsync();
                _logger.Log($"Order with ID {createdOrder.Id} created successfully.");
                return createdOrder;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order.");
                return null;
            }
        }

        public async Task<Order> UpdateOrderAsync(Order order)
        {
            try
            {
                order.UpdateTotalAmount();
                var updatedOrder = await _orderRepository.Update(order);
                await _orderRepository.SaveChangesAsync();
                _logger.Log($"Order with ID {updatedOrder.Id} updated successfully.");
                return updatedOrder;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order.");
                return null;
            }
        }

        public async Task<Order> DeleteOrderAsync(Guid id)
        {
            try
            {
                var order = await _orderQueries.GetById(id);
                if (order == null)
                {
                    _logger.LogError(new KeyNotFoundException($"Order with ID {id} not found."),
                        "Error deleting order.");
                    return null;
                }

                var deletedOrder = await _orderRepository.Delete(order);
                await _orderRepository.SaveChangesAsync();
                _logger.Log($"Order with ID {deletedOrder.Id} deleted successfully.");
                return deletedOrder;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting order.");
                return null;
            }
        }
    }
}