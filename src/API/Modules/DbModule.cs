using Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace API.Modules;

public static class DbModule
{
    public static async Task InitializeDb(this IHost host)
    {
        using var scope = host.Services.CreateScope();

        var provider = scope.ServiceProvider;
        var initializer = provider.GetRequiredService<ApplicationDbContextInitialiser>();

        await initializer.InitializeAsync();
    }
}