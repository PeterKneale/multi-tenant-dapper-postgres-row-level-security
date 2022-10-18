using System.Data;
using Dapper;
using Demo.Domain.PersonAggregate;

namespace Demo.Infrastructure.Repositories;

internal class PersonRepository : IPersonRepository
{
    private readonly IDbConnection _connection;
    private readonly IJsonSerializer _json;

    public PersonRepository(IDbConnection connection, IJsonSerializer json)
    {
        _connection = connection;
        _json = json;
    }

    public async Task<Person?> GetPerson(PersonId personId, CancellationToken token)
    {
        var sql = "select data from persons where id = @id";
        var parameters = new {id = personId.Id};
        var result = await _connection.QuerySingleOrDefaultAsync<string>(sql, parameters);
        if (result == null) return null;
        return _json.FromJson<Person>(result);
    }

    public async Task SavePerson(Person person, CancellationToken token)
    {
        var sql = "insert into persons (id, data) values (@id, @data::jsonb)";
        var parameters = new {id = person.Id.Id, data = _json.ToJson(person)};
        await _connection.ExecuteAsync(sql, parameters);
    }

    public async Task<IReadOnlyCollection<Person>> ListLicenced(CancellationToken token)
    {
        var sql = "select data from persons where (data ->> 'HasLicence')::bool = true";
        var results = await _connection.QueryAsync<string>(sql);
        return results.Select(x => _json.FromJson<Person>(x)).ToList();
    }

    public async Task<IReadOnlyCollection<Person>> ListUnlicenced(CancellationToken token)
    {
        var sql = "select data from persons where (data ->> 'HasLicence')::bool = false";
        var results = await _connection.QueryAsync<string>(sql);
        return results.Select(x => _json.FromJson<Person>(x)).ToList();
    }

    public async Task UpdatePerson(Person person, CancellationToken cancellationToken)
    {
        var sql = "update persons set data = @data::jsonb where id = @id";
        var json = _json.ToJson(person);
        var parameters = new {id = person.Id.Id, data = json};
        var result = await _connection.ExecuteAsync(sql, parameters);
        if (result != 1)
        {
            throw new Exception("Record not updated");
        }
    }
}