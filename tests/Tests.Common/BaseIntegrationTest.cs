using Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Tests.Common;    

public abstract class BaseIntegrationTest : IClassFixture<TestFactory>
{
    protected readonly ApplicationDbContext Context;

    protected BaseIntegrationTest(TestFactory factory)
    {
        var scope = factory.ServiceProvider.CreateScope();

        Context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    protected async Task<int> SaveChangesAsync()
    {
        var result = await Context.SaveChangesAsync();

        Context.ChangeTracker.Clear();

        return result;
    }
}