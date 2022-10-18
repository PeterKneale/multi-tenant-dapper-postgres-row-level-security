using Demo.Infrastructure.Repositories.Serialisation;

namespace Demo.Infrastructure.Repositories;

internal class CarRepository : ICarRepository
{
    private readonly IRepository _db;
    private readonly ITenantContext _context;

    public CarRepository(IRepository db, ITenantContext context)
    {
        _db = db;
        _context = context;
    }

    public async Task<Car?> Get(CarId carId, CancellationToken cancellationToken)
    {
        const string sql = "select data from cars where id = @id";
        var result = await _db.QuerySingleOrDefaultAsync(sql, new
        {
            id = carId.Id
        });
        return JsonHelper.ToObject<Car>(result);
    }

    public async Task<Car?> GetByRegistration(Registration registration, CancellationToken token)
    {
        const string sql = "select data from cars where registration = @registration";
        var result = await _db.QuerySingleOrDefaultAsync(sql, new
        {
            registration = registration.RegistrationNumber
        });
        return JsonHelper.ToObject<Car>(result);
    }
    
    public async Task<IEnumerable<Car>> List(CancellationToken cancellationToken)
    {
        const string sql = "select data from cars";
        var results = await _db.QueryAsync(sql, cancellationToken);
        return results
            .Select(result => JsonHelper.ToObject<Car>(result)!)
            .ToList();
    }

    public async Task Insert(Car car, CancellationToken cancellationToken)
    {
        const string sql = "insert into cars (id, tenant, registration, data) values (@id, @tenant, @registration, @data::jsonb)";
        var json = JsonHelper.ToJson(car);
        await _db.ExecuteAsync(sql, new
        {
            id = car.Id.Id,
            tenant = _context.CurrentTenant,
            registration = car.Registration?.RegistrationNumber,
            data = json
        });
    }

    public async Task Update(Car car, CancellationToken cancellationToken)
    {
        const string sql = "update cars set registration = @registration, data = @data::jsonb where id = @id";
        var result = await _db.ExecuteAsync(sql, new
        {
            id = car.Id.Id,
            registration = car.Registration?.RegistrationNumber,
            data = JsonHelper.ToJson(car)
        });
        if (result != 1)
        {
            throw new Exception("Record not updated");
        }
    }

}