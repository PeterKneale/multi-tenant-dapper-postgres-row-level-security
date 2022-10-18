using Demo.Infrastructure.Repositories;

namespace Demo.IntegrationTests.Fixtures;

public class ContainerFixture : IDisposable
{
    private readonly ServiceProvider _provider;

    public ContainerFixture()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                // Used by the tenants when using the app
                {"TenantConnectionString", "Host=localhost;Database=postgres;Username=tenant;Password=password"},
                // Used by the app when provisioning
                {"AdminConnectionString", "Host=localhost;Database=postgres;Username=postgres;Password=password"}
            })
            .Build();
        var services = new ServiceCollection();
        services.AddDemo(configuration);
        services.AddScoped<ITenantContext, TestTenantContext>();
        _provider = services.BuildServiceProvider();

        using var scope = _provider.CreateScope();
        var migrator = scope.ServiceProvider.GetRequiredService<MigrationExecutor>();
        migrator.ResetDatabase();
    }

    public IServiceProvider Provider => _provider;

    public void Dispose()
    {
        _provider.Dispose();
    }
}