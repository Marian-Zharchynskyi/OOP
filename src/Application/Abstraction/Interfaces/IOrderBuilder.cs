using Domain.Orders;
using Domain.Products;

namespace Application.Abstraction.Interfaces;

public interface IOrderBuilder
{
    IOrderBuilder AddProduct(Product product);
    IOrderBuilder CalculateTotal();
    Order Build();
}