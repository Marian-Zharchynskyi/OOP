
using Application.Abstraction.Interfaces;
using Application.Implementation;
using Application.Implementation.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ConfigureApplication
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();

        services.AddScoped<IOrderService, OrderService>();
        
        services.AddScoped<IOrderBuilder, OrderBuilder>();
    }
}