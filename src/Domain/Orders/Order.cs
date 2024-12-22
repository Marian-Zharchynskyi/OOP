using Domain.Products;

namespace Domain.Orders;

public class Order
{
    public Guid Id { get; set; }
    public List<Product>? Products { get; set; }
    public decimal TotalAmount { get; set; }

    public Order()
    {
        Id = Guid.NewGuid();
        Products = new List<Product>();
        TotalAmount = 0;
    }

    public void UpdateTotalAmount()
    {
        TotalAmount = Products.Sum(p => p.Price);
    }
}