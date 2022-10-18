using FluentMigrator;

namespace Demo.Infrastructure.Database;

[Migration(2)]
public class Migration2 : Migration
{
    const string Username = "tenant";
    const string Password = "password";
    const string Table = "cars";
    const string Column= "tenant";
    const string CurrentTenant= "current_setting('app.tenant')::VARCHAR";
    const string Policy = "tenant_isolation_policy";
    const string Permissions = "SELECT, UPDATE, INSERT, DELETE";

    public override void Up()
    {
        // Create a separate account for tenants to login with
        Execute.Sql($"CREATE USER {Username} LOGIN PASSWORD '{Password}';");
        
        // Give this tenant account access to the table 
        Execute.Sql($"GRANT {Permissions} ON {Table} TO {Username};");
        
        // This table should have row level security that ensure a tenant can only manage their own data
        Execute.Sql($"ALTER TABLE {Table} ENABLE ROW LEVEL SECURITY;");
        
        // Define the policy that will be applied
        Execute.Sql($"CREATE POLICY {Policy} ON {Table} FOR ALL TO {Username} USING ({Column} = {CurrentTenant});");
    }

    public override void Down()
    {
        // remove policy
        Execute.Sql($"DROP POLICY IF EXISTS {Policy} ON {Table};");
        // remove security
        Execute.Sql($"ALTER TABLE {Table} DISABLE ROW LEVEL SECURITY;");
        // revoke permission
        Execute.Sql($"REVOKE ALL ON {Table} FROM {Username};");
        // drop user
        Execute.Sql($"DROP USER {Username};");
    }
}