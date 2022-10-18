using Demo.Infrastructure.Repositories;

namespace Demo.IntegrationTests.Fixtures;

public static class ProviderExtensions
{
    public static async Task ExecuteCommand(this IServiceProvider provider, IRequest command, string tenant="DEMO")
    {
        using var scope = provider.CreateScope();
        SetContext(tenant, scope);
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        await mediator.Send(command);
    }

    public static async Task<T> ExecuteQuery<T>(this IServiceProvider provider, IRequest<T> query, string tenant="DEMO")
    {
        using var scope = provider.CreateScope();
        SetContext(tenant, scope);
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        return await mediator.Send(query);
    }
    private static void SetContext(string tenant, IServiceScope scope)
    {
        if (scope.ServiceProvider.GetRequiredService<ITenantContext>() is not TestTenantContext context)
        {
            throw new Exception("Cannot find context");
        }
        context.SetTestContext(tenant);
    }
}