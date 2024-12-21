
using Domain.Orders;

namespace Domain.Products;

public class Product
{
    public Guid Id { get; set; }
    public decimal Price { get; private set; }
    public string Name { get; private set; }
    public List<Order>? Orders { get; private set; }

    public Product(decimal price, string name)
    {
        Id = Guid.NewGuid();
        Price = price;
        Name = name;
    }
}
