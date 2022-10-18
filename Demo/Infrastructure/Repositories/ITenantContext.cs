namespace Demo.Infrastructure.Repositories;

internal interface ITenantContext
{
    string CurrentTenant { get; }
}
internal class TenantContext : ITenantContext
{
    public string CurrentTenant => "TENANT1";
}