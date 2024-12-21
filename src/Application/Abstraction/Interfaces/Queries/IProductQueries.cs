using Domain.Products;

namespace Application.Abstraction.Interfaces.Queries;

public interface IProductQueries
{
    Task<IReadOnlyList<Product>> GetAll();
    Task<Product> GetById(Guid id);
}