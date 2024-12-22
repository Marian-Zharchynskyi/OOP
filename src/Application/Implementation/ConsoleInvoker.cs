using Domain.Enums;
using Application.Abstraction.Interfaces;
using Domain.Products;

namespace Application.Implementation;

public class ConsoleInvoker
{
    private readonly IProductService _productService;
    private readonly IOrderService _orderService;
    private readonly IConsoleWrapper _consoleWrapper;
    private readonly INotifier _orderNotifier;
    private readonly IObserver _orderObserver;
    private readonly Dictionary<UserChoice, Func<Task>> _actionMap;

    public ConsoleInvoker(IProductService productService, IOrderService orderService, IConsoleWrapper consoleWrapper,
        INotifier orderNotifier,
        IObserver orderObserver)
    {
        _productService = productService;
        _orderService = orderService;
        _consoleWrapper = consoleWrapper;
        _orderNotifier = orderNotifier;
        _orderObserver = orderObserver;
        _orderNotifier.Attach(_orderObserver);

        _actionMap = new Dictionary<UserChoice, Func<Task>>
        {
            { UserChoice.GetAllProducts, GetAllProducts },
            { UserChoice.GetProductById, GetProductById },
            { UserChoice.CreateProduct, CreateProduct },
            { UserChoice.UpdateProduct, UpdateProduct },
            { UserChoice.DeleteProduct, DeleteProduct },
            { UserChoice.GetAllOrders, GetAllOrders },
            { UserChoice.GetOrderById, GetOrderById },
            { UserChoice.CreateOrder, CreateOrder },
            { UserChoice.UpdateOrder, UpdateOrder },
            { UserChoice.DeleteOrder, DeleteOrder },
            { UserChoice.AddProductsToOrder, AddProductsToOrder }
        };
    }

    public async Task Run()
    {
        bool isRunning = true;

        while (isRunning)
        {
            _consoleWrapper.WriteLine("Please select an action:");
            ShowMenu();

            if (Enum.TryParse(_consoleWrapper.ReadLine(), out UserChoice choice))
            {
                if (_actionMap.ContainsKey(choice))
                {
                    await _actionMap[choice]();
                }
                else if (choice == UserChoice.Exit)
                {
                    isRunning = false;
                }
                else
                {
                    _consoleWrapper.WriteLine("Invalid choice. Please try again.");
                }
            }
            else
            {
                _consoleWrapper.WriteLine("Invalid input. Please enter a number corresponding to the action.");
            }

            if (isRunning)
            {
                _consoleWrapper.WriteLine("Press Enter to continue...");
                _consoleWrapper.ReadLine();
            }
        }
    }

    private void ShowMenu()
    {
        foreach (UserChoice choice in Enum.GetValues(typeof(UserChoice)))
        {
            _consoleWrapper.WriteLine($"{(int)choice}. {choice}");
        }
    }

    private async Task GetAllProducts()
    {
        var products = await _productService.GetAllProductsAsync();
        if (products.Count == 0)
        {
            _consoleWrapper.WriteLine("No products found.");
            return;
        }

        foreach (var product in products)
        {
            _consoleWrapper.WriteLine($"ID: {product.Id}, Name: {product.Name}, Price: {product.Price}");
        }
    }

    private async Task GetProductById()
    {
        _consoleWrapper.WriteLine("Enter Product ID: ");
        if (Guid.TryParse(_consoleWrapper.ReadLine(), out Guid id))
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                _consoleWrapper.WriteLine("Product not found.");
                return;
            }

            _consoleWrapper.WriteLine($"ID: {product.Id}, Name: {product.Name}, Price: {product.Price}");
        }
        else
        {
            _consoleWrapper.WriteLine("Invalid ID format.");
        }
    }

    private async Task CreateProduct()
    {
        _consoleWrapper.WriteLine("Enter Product Name: ");
        string name = _consoleWrapper.ReadLine();

        _consoleWrapper.WriteLine("Enter Product Price: ");
        if (decimal.TryParse(_consoleWrapper.ReadLine(), out decimal price))
        {
            var product = new Product(price, name);

            var createdProduct = await _productService.CreateProductAsync(product);
            if (createdProduct != null)
            {
                _consoleWrapper.WriteLine($"Product created with ID: {createdProduct.Id}");

                _orderNotifier.Notify($"Product created with ID: {createdProduct.Id}");
            }
            else
            {
                _consoleWrapper.WriteLine("Error creating product.");
            }
        }
        else
        {
            _consoleWrapper.WriteLine("Invalid price format.");
        }
    }

    private async Task UpdateProduct()
    {
        _consoleWrapper.WriteLine("Enter Product ID to update: ");
        if (Guid.TryParse(_consoleWrapper.ReadLine(), out Guid id))
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                _consoleWrapper.WriteLine("Product not found.");
                return;
            }

            _consoleWrapper.WriteLine("Enter new name for the product (leave blank to keep current): ");
            string newName = _consoleWrapper.ReadLine();
            if (!string.IsNullOrEmpty(newName))
            {
                product.Name = newName;
            }

            _consoleWrapper.WriteLine("Enter new price for the product (leave blank to keep current): ");
            if (decimal.TryParse(_consoleWrapper.ReadLine(), out decimal newPrice))
            {
                product.Price = newPrice;
            }

            var updatedProduct = await _productService.UpdateProductAsync(product);
            if (updatedProduct != null)
            {
                _consoleWrapper.WriteLine(
                    $"Product updated. New Name: {updatedProduct.Name}, New Price: {updatedProduct.Price}");
            }
            else
            {
                _consoleWrapper.WriteLine("Error updating product.");
            }
        }
        else
        {
            _consoleWrapper.WriteLine("Invalid ID format.");
        }
    }

    private async Task DeleteProduct()
    {
        _consoleWrapper.WriteLine("Enter Product ID to delete: ");
        if (Guid.TryParse(_consoleWrapper.ReadLine(), out Guid id))
        {
            var deletedProduct = await _productService.DeleteProductAsync(id);
            if (deletedProduct != null)
            {
                _consoleWrapper.WriteLine($"Product with ID {deletedProduct.Id} deleted.");
            }
            else
            {
                _consoleWrapper.WriteLine("Error deleting product or product not found.");
            }
        }
        else
        {
            _consoleWrapper.WriteLine("Invalid ID format.");
        }
    }

    private async Task GetAllOrders()
    {
        var orders = await _orderService.GetAllOrdersAsync();
        if (orders.Count == 0)
        {
            _consoleWrapper.WriteLine("No orders found.");
            return;
        }

        foreach (var order in orders)
        {
            _consoleWrapper.WriteLine($"Order ID: {order.Id}, Total Amount: {order.TotalAmount}");
        }
    }

    private async Task GetOrderById()
    {
        _consoleWrapper.WriteLine("Enter Order ID: ");
        if (Guid.TryParse(_consoleWrapper.ReadLine(), out Guid id))
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                _consoleWrapper.WriteLine("Order not found.");
                return;
            }

            _consoleWrapper.WriteLine($"Order ID: {order.Id}, Total Amount: {order.TotalAmount}");
        }
        else
        {
            _consoleWrapper.WriteLine("Invalid ID format.");
        }
    }

    private async Task CreateOrder()
    {
        _consoleWrapper.WriteLine("Enter Product IDs for the order (comma separated): ");
        var ids = _consoleWrapper.ReadLine()?.Split(',');
        if (ids != null)
        {
            var products = new List<Product>();
            foreach (var id in ids)
            {
                if (Guid.TryParse(id.Trim(), out Guid productId))
                {
                    var product = await _productService.GetProductByIdAsync(productId);
                    if (product != null)
                    {
                        products.Add(product);
                    }
                }
            }

            var order = await _orderService.CreateOrderAsync(products);
            if (order != null)
            {
                _consoleWrapper.WriteLine($"Order created with ID: {order.Id}");

                _orderNotifier.Notify($"Order created with ID: {order.Id}");
            }
            else
            {
                _consoleWrapper.WriteLine("Error creating order.");
            }
        }
        else
        {
            _consoleWrapper.WriteLine("Invalid input.");
        }
    }

    private async Task UpdateOrder()
    {
        _consoleWrapper.WriteLine("Enter Order ID to update: ");
        if (Guid.TryParse(_consoleWrapper.ReadLine(), out Guid id))
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                _consoleWrapper.WriteLine("Order not found.");
                return;
            }

            order.UpdateTotalAmount();
            var updatedOrder = await _orderService.UpdateOrderAsync(order);
            if (updatedOrder != null)
            {
                _consoleWrapper.WriteLine($"Order updated. Total Amount: {updatedOrder.TotalAmount}");
            }
            else
            {
                _consoleWrapper.WriteLine("Error updating order.");
            }
        }
        else
        {
            _consoleWrapper.WriteLine("Invalid ID format.");
        }
    }

    private async Task DeleteOrder()
    {
        _consoleWrapper.WriteLine("Enter Order ID to delete: ");
        if (Guid.TryParse(_consoleWrapper.ReadLine(), out Guid id))
        {
            var deletedOrder = await _orderService.DeleteOrderAsync(id);
            if (deletedOrder != null)
            {
                _consoleWrapper.WriteLine($"Order with ID {deletedOrder.Id} deleted.");
            }
            else
            {
                _consoleWrapper.WriteLine("Error deleting order or order not found.");
            }
        }
        else
        {
            _consoleWrapper.WriteLine("Invalid ID format.");
        }
    }

    private async Task AddProductsToOrder()
    {
        _consoleWrapper.WriteLine("Enter Order ID to add products: ");
        if (Guid.TryParse(_consoleWrapper.ReadLine(), out Guid orderId))
        {
            _consoleWrapper.WriteLine("Enter Product IDs to add, separated by commas: ");
            var productIdsInput = _consoleWrapper.ReadLine();
            var productIds = productIdsInput.Split(',')
                .Select(id => Guid.TryParse(id, out var productId) ? productId : Guid.Empty)
                .Where(id => id != Guid.Empty)
                .ToList();
            var products = new List<Product>();
            foreach (var productId in productIds)
            {
                var product = await _productService.GetProductByIdAsync(productId);
                if (product != null)
                {
                    products.Add(product);
                }
                else
                {
                    _consoleWrapper.WriteLine($"Product with ID {productId} not found.");
                }
            }

            if (products.Any())
            {
                var updatedOrder = await _orderService.AddProductsToOrderAsync(orderId, products);
                if (updatedOrder != null)
                {
                    _consoleWrapper.WriteLine(
                        $"Successfully added products to order {orderId}. Total amount: {updatedOrder.TotalAmount}");
                }
                else
                {
                    _consoleWrapper.WriteLine("Error adding products to the order.");
                }
            }
            else
            {
                _consoleWrapper.WriteLine("No valid products to add.");
            }
        }
        else
        {
            _consoleWrapper.WriteLine("Invalid Order ID format.");
        }
    }
}