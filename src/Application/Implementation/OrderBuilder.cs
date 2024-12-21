using Application.Abstraction.Interfaces;
using Domain.Orders;
using Domain.Products;

namespace Application.Implementation;

public class OrderBuilder : IOrderBuilder
{
    private Order _order;

    public OrderBuilder()
    {
        _order = new Order();
    }

    public IOrderBuilder AddProduct(Product product)
    {
        _order.Products.Add(product);
        return this;
    }

    public IOrderBuilder CalculateTotal()
    {
        _order.UpdateTotalAmount(); 
        return this;
    }

    public Order Build()
    {
        return _order;
    }
}
