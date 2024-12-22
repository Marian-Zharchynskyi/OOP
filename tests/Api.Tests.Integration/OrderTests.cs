using Application.Abstraction.Interfaces;
using Application.Abstraction.Interfaces.Repositories;
using Application.Abstraction.Interfaces.Queries;
using Application.Implementation.Services;
using Domain.Orders;
using Domain.Products;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;
using Tests.Data;  

namespace Api.Tests.Integration
{
    public class OrderTests
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderQueries _orderQueries;
        private readonly ILogger _logger;
        private readonly IOrderBuilder _orderBuilder;
        private readonly OrderService _orderService;
        private readonly Product _existingProduct;

        public OrderTests()
        {
            _orderRepository = Substitute.For<IOrderRepository>();
            _orderQueries = Substitute.For<IOrderQueries>();
            _logger = Substitute.For<ILogger>();
            _orderBuilder = Substitute.For<IOrderBuilder>();
            _orderService = new OrderService(_orderRepository, _orderQueries, _logger, _orderBuilder);

            _existingProduct = ProductData.Product;
        }

        [Fact]
        public async Task GetAllOrdersAsync_ReturnsOrders_WhenSuccessful()
        {
            // Arrange
            var orders = new List<Order> { new Order { Id = Guid.NewGuid(), TotalAmount = 100 } };
            _orderQueries.GetAll().Returns(orders);

            // Act
            var result = await _orderService.GetAllOrdersAsync();

            // Assert
            result.Should().NotBeEmpty();
            result.Should().HaveCount(1);
            _logger.Received(1).Log(Arg.Any<string>());
        }

        [Fact]
        public async Task GetAllOrdersAsync_ReturnsEmptyList_WhenErrorOccurs()
        {
            // Arrange
            _orderQueries.GetAll().Throws(new Exception("Error"));

            // Act
            var result = await _orderService.GetAllOrdersAsync();

            // Assert
            result.Should().BeEmpty();
            _logger.Received(1).LogError(Arg.Any<Exception>(), Arg.Any<string>());
        }

        [Fact]
        public async Task GetOrderByIdAsync_ReturnsOrder_WhenOrderExists()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order { Id = orderId, TotalAmount = 100 };
            _orderQueries.GetById(orderId).Returns(order);

            // Act
            var result = await _orderService.GetOrderByIdAsync(orderId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(orderId);
            _logger.Received(1).Log(Arg.Any<string>());
        }

        [Fact]
        public async Task GetOrderByIdAsync_ReturnsNull_WhenOrderDoesNotExist()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            _orderQueries.GetById(orderId).Returns((Order)null);

            // Act
            var result = await _orderService.GetOrderByIdAsync(orderId);

            // Assert
            result.Should().BeNull();
            _logger.Received(1).LogError(Arg.Any<Exception>(), Arg.Any<string>());
        }

        [Fact]
        public async Task CreateOrderAsync_CreatesOrderSuccessfully()
        {
            // Arrange
            var products = new List<Product> { _existingProduct };
            var order = new Order { Id = Guid.NewGuid(), TotalAmount = 100 };
            _orderBuilder.Build().Returns(order);
            _orderRepository.Add(order).Returns(order);

            // Act
            var result = await _orderService.CreateOrderAsync(products);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(order.Id);
            _logger.Received(1).Log(Arg.Any<string>());
        }

        [Fact]
        public async Task CreateOrderAsync_ReturnsNull_WhenErrorOccurs()
        {
            // Arrange
            var products = new List<Product> { _existingProduct };
            _orderRepository.Add(Arg.Any<Order>()).Throws(new Exception("Error"));

            // Act
            var result = await _orderService.CreateOrderAsync(products);

            // Assert
            result.Should().BeNull();
            _logger.Received(1).LogError(Arg.Any<Exception>(), Arg.Any<string>());
        }

        [Fact]
        public async Task UpdateOrderAsync_UpdatesOrderSuccessfully()
        {
            // Arrange
            var order = new Order { Id = Guid.NewGuid(), TotalAmount = 100 };
            _orderRepository.Update(order).Returns(order);

            // Act
            var result = await _orderService.UpdateOrderAsync(order);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(order.Id);
            _logger.Received(1).Log(Arg.Any<string>());
        }

        [Fact]
        public async Task UpdateOrderAsync_ReturnsNull_WhenErrorOccurs()
        {
            // Arrange
            var order = new Order { Id = Guid.NewGuid(), TotalAmount = 100 };
            _orderRepository.Update(order).Throws(new Exception("Error"));

            // Act
            var result = await _orderService.UpdateOrderAsync(order);

            // Assert
            result.Should().BeNull();
            _logger.Received(1).LogError(Arg.Any<Exception>(), Arg.Any<string>());
        }

        [Fact]
        public async Task DeleteOrderAsync_DeletesOrderSuccessfully()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order { Id = orderId, TotalAmount = 100 };
            _orderQueries.GetById(orderId).Returns(order);
            _orderRepository.Delete(order).Returns(order);

            // Act
            var result = await _orderService.DeleteOrderAsync(orderId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(orderId);
            _logger.Received(1).Log(Arg.Any<string>());
        }

        [Fact]
        public async Task DeleteOrderAsync_ReturnsNull_WhenOrderDoesNotExist()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            _orderQueries.GetById(orderId).Returns((Order)null);

            // Act
            var result = await _orderService.DeleteOrderAsync(orderId);

            // Assert
            result.Should().BeNull();
            _logger.Received(1).LogError(Arg.Any<Exception>(), Arg.Any<string>());
        }

        [Fact]
        public async Task AddProductsToOrderAsync_AddsProductsSuccessfully()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order { Id = orderId, TotalAmount = 100, Products = new List<Product>() };
            var products = new List<Product> { _existingProduct };

            _orderQueries.GetById(orderId).Returns(order);
            _orderRepository.Update(order).Returns(order);

            // Act
            var result = await _orderService.AddProductsToOrderAsync(orderId, products);

            // Assert
            result.Should().NotBeNull();
            result.Products.Should().Contain(_existingProduct);
            _logger.Received(1).Log(Arg.Any<string>());
        }

        [Fact]
        public async Task AddProductsToOrderAsync_ReturnsNull_WhenOrderDoesNotExist()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var products = new List<Product> { _existingProduct };
            _orderQueries.GetById(orderId).Returns((Order)null);

            // Act
            var result = await _orderService.AddProductsToOrderAsync(orderId, products);

            // Assert
            result.Should().BeNull();
            _logger.Received(1).LogError(Arg.Any<Exception>(), Arg.Any<string>());
        }
    }
}
