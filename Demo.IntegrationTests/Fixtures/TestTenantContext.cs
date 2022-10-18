using Demo.Infrastructure.Repositories;

namespace Demo.IntegrationTests.Fixtures;

public class TestTenantContext : ITenantContext
{
    private string? _context;

    public void SetTestContext(string context)
    {
        _context = context;

    }
    public string CurrentTenant => _context ?? throw new Exception("Tenant Context hasn't been set for test");
}