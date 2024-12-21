using Domain.Products;

namespace Application.Abstraction.Interfaces;

public interface IProductService
{
    Task<IReadOnlyList<Product>> GetAllProductsAsync();
    Task<Product> GetProductByIdAsync(Guid id);
    Task<Product> CreateProductAsync(Product product);
    Task<Product> UpdateProductAsync(Product product);
    Task<Product> DeleteProductAsync(Guid id);
}