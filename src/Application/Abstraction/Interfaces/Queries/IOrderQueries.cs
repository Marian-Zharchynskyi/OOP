using Domain.Orders;

namespace Application.Abstraction.Interfaces.Queries;

public interface IOrderQueries
{
    Task<IReadOnlyList<Order>> GetAll();
    Task<Order> GetById(Guid id);
}