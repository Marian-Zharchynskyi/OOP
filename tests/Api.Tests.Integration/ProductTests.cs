using Application.Abstraction.Interfaces;
using Application.Abstraction.Interfaces.Queries;
using Application.Abstraction.Interfaces.Repositories;
using Application.Implementation.Services;
using Domain.Products;
using FluentAssertions;
using NSubstitute;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration;

public class ProductServiceTests
{
    private readonly IProductService _productService;
    private readonly IProductRepository _mockProductRepository;
    private readonly IProductQueries _mockProductQueries;
    private readonly ILogger _mockLogger;

    private readonly Product _existingProduct;

    public ProductServiceTests()
    {
        // Arrange
        _mockProductRepository = Substitute.For<IProductRepository>();
        _mockProductQueries = Substitute.For<IProductQueries>();
        _mockLogger = Substitute.For<ILogger>();

        // Act
        _productService = new ProductService(_mockProductRepository, _mockProductQueries, _mockLogger);

        // Assert
        _existingProduct = ProductData.Product;
    }

    [Fact]
    public async Task UpdateProductAsync_ShouldReturnUpdatedProduct_WhenSuccessful()
    {
        // Arrange
        var productId = _existingProduct.Id;
    
        _mockProductQueries.GetById(productId).Returns(_existingProduct);
    
        _mockProductRepository.Update(Arg.Any<Product>()).Returns(_existingProduct);

        // Act
        var result = await _productService.UpdateProductAsync(_existingProduct);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(_existingProduct.Id);
        result.Name.Should().Be(_existingProduct.Name);
        result.Price.Should().Be(_existingProduct.Price);

        _mockLogger.Received().Log(Arg.Is<string>(msg => msg.Contains("updated successfully")));
    }


    [Fact]
    public async Task UpdateProductAsync_ShouldReturnNull_WhenProductNotFound()
    {
        // Arrange
        var productId = _existingProduct.Id;
        _mockProductQueries.GetById(productId).Returns((Product)null);

        // Act
        var result = await _productService.UpdateProductAsync(_existingProduct);

        // Assert
        result.Should().BeNull();
        _mockLogger.Received().LogError(Arg.Any<KeyNotFoundException>(), Arg.Any<string>());
    }


    [Fact]
    public async Task DeleteProductAsync_ShouldReturnDeletedProduct_WhenSuccessful()
    {
        // Arrange
        var productId = _existingProduct.Id;
        _mockProductQueries.GetById(productId).Returns(_existingProduct);
        _mockProductRepository.Delete(Arg.Any<Product>()).Returns(_existingProduct);

        // Act
        var result = await _productService.DeleteProductAsync(productId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(productId);

        _mockLogger.Received().Log(Arg.Any<string>());
    }

    [Fact]
    public async Task DeleteProductAsync_ShouldReturnNull_WhenProductNotFound()
    {
        // Arrange
        var productId = _existingProduct.Id;
        _mockProductQueries.GetById(productId).Returns((Product)null); // Продукт не знайдений

        // Act
        var result = await _productService.DeleteProductAsync(productId);

        // Assert
        result.Should().BeNull();
        _mockLogger.Received().LogError(Arg.Any<KeyNotFoundException>(), Arg.Any<string>());
    }

    [Fact]
    public async Task GetAllProductsAsync_ShouldReturnProductList_WhenSuccessful()
    {
        // Arrange
        var productList = new List<Product> { _existingProduct };
        _mockProductQueries.GetAll().Returns(productList);

        // Act
        var result = await _productService.GetAllProductsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().ContainSingle();
        result.First().Id.Should().Be(_existingProduct.Id);
    }

    [Fact]
    public async Task GetProductByIdAsync_ShouldReturnProduct_WhenFound()
    {
        // Arrange
        var productId = _existingProduct.Id;
        _mockProductQueries.GetById(productId).Returns(_existingProduct);

        // Act
        var result = await _productService.GetProductByIdAsync(productId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(productId);
    }

    [Fact]
    public async Task GetProductByIdAsync_ShouldReturnNull_WhenNotFound()
    {
        // Arrange
        var productId = _existingProduct.Id;
        _mockProductQueries.GetById(productId).Returns((Product)null);

        // Act
        var result = await _productService.GetProductByIdAsync(productId);

        // Assert
        result.Should().BeNull();
        _mockLogger.Received().LogError(Arg.Any<KeyNotFoundException>(), Arg.Any<string>());
    }

    [Fact]
    public async Task CreateProductAsync_ShouldReturnCreatedProduct_WhenSuccessful()
    {
        // Arrange
        _mockProductRepository.Add(Arg.Any<Product>()).Returns(_existingProduct);

        // Act
        var result = await _productService.CreateProductAsync(_existingProduct);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(_existingProduct.Id);
        result.Name.Should().Be(_existingProduct.Name);
        result.Price.Should().Be(_existingProduct.Price);
        _mockLogger.Received().Log(Arg.Any<string>());
    }

    [Fact]
    public async Task CreateProductAsync_ShouldReturnNull_WhenErrorOccurs()
    {
        // Arrange
        _mockProductRepository.Add(Arg.Any<Product>()).Returns((Product)null);

        // Act
        var result = await _productService.CreateProductAsync(_existingProduct);

        // Assert
        result.Should().BeNull();
        _mockLogger.Received().LogError(Arg.Any<Exception>(), Arg.Any<string>());
    }
}
