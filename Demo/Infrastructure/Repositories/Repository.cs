using Dapper;

namespace Demo.Infrastructure.Repositories;

internal interface IRepository
{
    Task<int> ExecuteAsync(string sql, object parameters);
    Task<string?> QuerySingleOrDefaultAsync(string sql, object parameters);
    Task<IEnumerable<string>> QueryAsync(string sql, object parameters);
}
internal class Repository : IRepository
{
    private readonly IDbConnection _connection;
    private readonly ITenantContext _context;
    
    public Repository(IDbConnection connection, ITenantContext context)
    {
        _connection = connection;
        _context = context;
    }

    public async Task<int> ExecuteAsync(string sql, object parameters)
    {
        _connection.Open();
        await SetCurrentTenant();
        var result = await _connection.ExecuteAsync(sql, parameters);
        _connection.Close();
        return result;
    }
    public async Task<IEnumerable<string>> QueryAsync(string sql, object parameters)
    {
        _connection.Open();
        await SetCurrentTenant();
        var results = await _connection.QueryAsync<string>(sql, parameters);
        _connection.Close();
        return results;
    }
    public async Task<IEnumerable<string>> QueryAsync(string sql)
    {
        _connection.Open();
        await SetCurrentTenant();
        var results = await _connection.QueryAsync<string>(sql);
        _connection.Close();
        return results;
    }

    public async Task<string?> QuerySingleOrDefaultAsync(string sql, object parameters)
    {
        _connection.Open();
        await SetCurrentTenant();
        var result =  await _connection.QuerySingleOrDefaultAsync<string?>(sql, parameters);
        _connection.Close();
        return result;
    }

    private async Task SetCurrentTenant()
    {
        var tenant = _context.CurrentTenant;
        var sql = $"SET app.tenant = '{tenant}';";
        await _connection.ExecuteAsync(sql);
    }
}