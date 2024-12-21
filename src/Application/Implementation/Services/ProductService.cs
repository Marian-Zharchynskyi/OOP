using Application.Abstraction.Interfaces;
using Application.Abstraction.Interfaces.Queries;
using Application.Abstraction.Interfaces.Repositories;
using Domain.Products;

namespace Application.Implementation.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductQueries _productQueries;
        private readonly ILogger _logger;

        public ProductService(IProductRepository productRepository, IProductQueries productQueries, ILogger logger)
        {
            _productRepository = productRepository;
            _productQueries = productQueries;
            _logger = logger;
        }

        public async Task<IReadOnlyList<Product>> GetAllProductsAsync()
        {
            try
            {
                var products = await _productQueries.GetAll();
                _logger.Log("Fetched all products successfully.");
                return products;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all products.");
                return new List<Product>();
            }
        }

        public async Task<Product> GetProductByIdAsync(Guid id)
        {
            try
            {
                var product = await _productQueries.GetById(id);
                if (product == null)
                {
                    _logger.LogError(new KeyNotFoundException($"Product with ID {id} not found."),
                        "Error fetching product by ID.");
                    return null;
                }

                _logger.Log($"Fetched product with ID {id} successfully.");
                return product;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching product by ID.");
                return null;
            }
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            try
            {
                var createdProduct = await _productRepository.Add(product);
                await _productRepository.SaveChangesAsync();
                _logger.Log($"Product with ID {createdProduct.Id} created successfully.");
                return createdProduct;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product.");
                return null;
            }
        }

        public async Task<Product> UpdateProductAsync(Product product)
        {
            try
            {
                var updatedProduct = await _productRepository.Update(product);
                await _productRepository.SaveChangesAsync();
                _logger.Log($"Product with ID {updatedProduct.Id} updated successfully.");
                return updatedProduct;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product.");
                return null;
            }
        }

        public async Task<Product> DeleteProductAsync(Guid id)
        {
            try
            {
                var product = await _productQueries.GetById(id);
                if (product == null)
                {
                    _logger.LogError(new KeyNotFoundException($"Product with ID {id} not found."),
                        "Error deleting product.");
                    return null;
                }

                var deletedProduct = await _productRepository.Delete(product);
                await _productRepository.SaveChangesAsync();
                _logger.Log($"Product with ID {deletedProduct.Id} deleted successfully.");
                return deletedProduct;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product.");
                return null;
            }
        }
    }
}