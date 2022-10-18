# Demo of a multi-tenant application using Dapper and Postgres with Row level security

## Configuring security policy

```sql
// Create a separate account for tenants to login with
Execute.Sql($"CREATE USER {Username} LOGIN PASSWORD '{Password}';");

// Give this tenant account access to the table 
Execute.Sql($"GRANT SELECT, UPDATE, INSERT, DELETE ON {Table} TO {Username};");

// This table should have row level security that ensure a tenant can only manage their own data
Execute.Sql($"ALTER TABLE {Table} ENABLE ROW LEVEL SECURITY;");

// Define the policy that will be applied
Execute.Sql($"CREATE POLICY {Policy} ON {Table} FOR ALL TO {Username} USING (current_setting('app.tenant')::VARCHAR);");
```

## Interacting with security policy

```sql
// The the tenant context for this connection
SET app.tenant = '{Tenant}'

// Only this rows belonging to this tenant will be returned
SELECT * FROM {Table}
```
