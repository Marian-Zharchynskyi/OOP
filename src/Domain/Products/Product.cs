
using Domain.Orders;

namespace Domain.Products;

public class Product
{
    public Guid Id { get; set; }
    public decimal Price { get;  set; }
    public string Name { get;  set; }
    public List<Order>? Orders { get; }

    public Product(decimal price, string name)
    {
        Id = Guid.NewGuid();
        Price = price;
        Name = name;
    }
}
