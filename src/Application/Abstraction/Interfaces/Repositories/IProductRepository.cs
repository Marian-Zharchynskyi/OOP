using Domain.Products;

namespace Application.Abstraction.Interfaces.Repositories;

public interface IProductRepository
{
    Task<Product> Add(Product product);
    Task<Product> Update(Product product);
    Task<Product> Delete(Product product);
}