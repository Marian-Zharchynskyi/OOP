using API;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Testcontainers.PostgreSql;
using Xunit;

namespace Tests.Common;

public class TestFactory : IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("OOPFinalProject")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();
    public IServiceProvider ServiceProvider { get; private set; }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        var host = await HostCreator.CreateHost([], builder =>
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.Test.json", optional: false, reloadOnChange: true);
            });

            builder.ConfigureServices((context, services) =>
            {
                services.RemoveServiceByType(typeof(DbContextOptions<ApplicationDbContext>));

                var dataSourceBuilder = new NpgsqlDataSourceBuilder(_dbContainer.GetConnectionString());
                dataSourceBuilder.EnableDynamicJson();
                var dataSource = dataSourceBuilder.Build();

                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseNpgsql(dataSource)
                        .UseSnakeCaseNamingConvention());
            });
        });

        ServiceProvider = host.Services;
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync().AsTask();
    }
}

public static class TestFactoryExtensions
{
    public static void RemoveServiceByType(this IServiceCollection services, Type serviceType)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == serviceType);

        if (descriptor is not null)
            services.Remove(descriptor);
    }
}