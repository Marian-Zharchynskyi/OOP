using Application.Abstraction.Interfaces;
using Application.Abstraction.Interfaces.Repositories;
using Application.Abstraction.Interfaces.Queries;
using Domain.Orders;
using Domain.Products;

namespace Application.Implementation.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderQueries _orderQueries;
        private readonly ILogger _logger;
        private readonly IOrderBuilder _orderBuilder;

        public OrderService(IOrderRepository orderRepository, IOrderQueries orderQueries, ILogger logger,
            IOrderBuilder orderBuilder)
        {
            _orderRepository = orderRepository;
            _orderQueries = orderQueries;
            _logger = logger;
            _orderBuilder = orderBuilder;
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

        public async Task<Order> CreateOrderAsync(List<Product> products)
        {
            try
            {
                foreach (var product in products)
                {
                    _orderBuilder.AddProduct(product);
                }

                _orderBuilder.CalculateTotal();

                var order = _orderBuilder.Build();

                var createdOrder = await _orderRepository.Add(order);

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
                _logger.Log($"Order with ID {deletedOrder.Id} deleted successfully.");
                return deletedOrder;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting order.");
                return null;
            }
        }

        public async Task<Order> AddProductsToOrderAsync(Guid orderId, List<Product> products)
        {
            try
            {
                var order = await _orderQueries.GetById(orderId);
                if (order == null)
                {
                    _logger.LogError(new KeyNotFoundException($"Order with ID {orderId} not found."),
                        "Error adding products to order.");
                    return null;
                }

                foreach (var product in products)
                {
                    if (!order.Products.Contains(product))
                    {
                        order.Products.Add(product);
                    }
                }

                order.UpdateTotalAmount();

                var updatedOrder = await _orderRepository.Update(order);
                _logger.Log($"Products added to order with ID {updatedOrder.Id} successfully.");
                return updatedOrder;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding products to order.");
                return null;
            }
        }
    }
}