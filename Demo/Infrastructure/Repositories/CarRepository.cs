using System.Data;
using Dapper;
using Demo.Domain.CarAggregate;
using Demo.Domain.PersonAggregate;

namespace Demo.Infrastructure.Repositories;

internal class CarRepository : ICarRepository
{
    private readonly IDbConnection _connection;
    private readonly IJsonSerializer _json;

    public CarRepository(IDbConnection connection, IJsonSerializer json)
    {
        _connection = connection;
        _json = json;
    }

    public async Task<Car?> GetCar(CarId carId, CancellationToken token)
    {
        var sql = "select data from cars where id = @id";
        var parameters = new {id = carId.Id};
        var result = await _connection.QuerySingleOrDefaultAsync<string>(sql, parameters);
        if (result == null) return null;
        return _json.FromJson<Car>(result);
    }

    public async Task SaveCar(Car car, CancellationToken token)
    {
        var sql = "insert into cars (id, registration, data) values (@id, @registration, @data::jsonb)";
        var json = _json.ToJson(car);
        var parameters = new {id = car.Id.Id, registration = car.Registration?.RegistrationNumber, data = json};
        await _connection.ExecuteAsync(sql, parameters);
    }

    public async Task UpdateCar(Car car, CancellationToken cancellationToken)
    {
        var sql = "update cars set registration = @registration, data = @data::jsonb where id = @id";
        var json = _json.ToJson(car);
        var parameters = new {id = car.Id.Id, registration = car.Registration?.RegistrationNumber, data = json};
        var result = await _connection.ExecuteAsync(sql, parameters);
        if (result != 1)
        {
            throw new Exception("Record not updated");
        }
    }

    public async Task<Car?> GetByRegistration(Registration registration, CancellationToken token)
    {
        var sql = "select data from cars where registration = @registration";
        var parameters = new {registration = registration.RegistrationNumber};
        var result = await _connection.QuerySingleOrDefaultAsync<string>(sql, parameters);
        if (result == null) return null;
        return _json.FromJson<Car>(result);
    }

    public async Task<IReadOnlyCollection<Car>> ListByOwner(PersonId ownerId, CancellationToken token)
    {
        var sql = "select data from cars where (data -> 'Owner' ->> 'Id')::uuid = @id;";
        var parameters = new {id = ownerId.Id};
        var results = await _connection.QueryAsync<string>(sql, parameters);
        return results.Select(x => _json.FromJson<Car>(x)).ToList();
    }
}