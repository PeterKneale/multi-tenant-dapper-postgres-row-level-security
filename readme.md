# Dotnet + Postgres, Jsonb and Dapper
Demonstrates how to execute various use cases against a postgres database, storing aggregates in a jsonb column, indexing data within that jsonb data and querying against it. Also demonstrates the promotion of a json attribute to its own column for queries, joins or further indexing.

## Database schema
- `cars` stores its data in a jsonb column
- `cars` has the property 'registration` promoted to  a column for queryies or joins
- `persons` stores its data in a jsonb column
```cs
public class Migration1 : Migration
{
    public override void Up()
    {
        Create.Table("cars")
            .WithColumn("id").AsGuid().NotNullable().PrimaryKey()
            .WithColumn("registration").AsString().Nullable().Unique()
            .WithColumn("data").AsCustom("jsonb").NotNullable();
                    
        // Index Car.Owner.Id for use cases `ListCarsByOwner`
        Execute.Sql("create index idx_car_owner_id on cars (((data->'Owner'->>'Id')::uuid));");

        Create.Table("persons")
            .WithColumn("id").AsGuid().NotNullable().PrimaryKey()
            .WithColumn("data").AsCustom("jsonb").NotNullable();

        // Index Person.HasLicence for use cases `ListLicencedPerson` and `ListUnlicensedPerson`
        Execute.Sql("create index idx_person_has_licence on persons (((data ->> 'HasLicence')::bool));");
    }
```

## Use cases

- Get By Id
```cs
public async Task<Car?> GetCar(CarId carId, CancellationToken token)
{
    var sql = "select data from cars where id = @id";
    var parameters = new {id = carId.Id};
    var result = await _connection.QuerySingleOrDefaultAsync<string>(sql, parameters);
    if (result == null) return null;
    return _json.FromJson<Car>(result);
}
```

- Save, promoting a field on the domain object to a column in the table
```cs
public async Task SaveCar(Car car, CancellationToken token)
{
    var sql = "insert into cars (id, registration, data) values (@id, @registration, @data::jsonb)";
    var json = _json.ToJson(car);
    var parameters = new {id = car.Id.Id, registration = car.Registration?.RegistrationNumber, data = json};
    await _connection.ExecuteAsync(sql, parameters);
}
```

- Update, promoting a field on the domain object to a column in the table
```cs
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
```

- Query, by a column in the table
```cs
public async Task<Car?> GetByRegistration(Registration registration, CancellationToken token)
{
    var sql = "select data from cars where registration = @registration";
    var parameters = new {registration = registration.RegistrationNumber};
    var result = await _connection.QuerySingleOrDefaultAsync<string>(sql, parameters);
    if (result == null) return null;
    return _json.FromJson<Car>(result);
}
```

- Query, by a json attribute which is a uuid, with an index on it
```cs
public async Task<IReadOnlyCollection<Car>> ListByOwner(PersonId ownerId, CancellationToken token)
{
    var sql = "select data from cars where (data -> 'Owner' ->> 'Id')::uuid = @id;";
    var parameters = new {id = ownerId.Id};
    var results = await _connection.QueryAsync<string>(sql, parameters);
    return results.Select(x => _json.FromJson<Car>(x)).ToList();
}
```
    
- Query, by a json attribute which is a boolean, with an index on it
```cs
public async Task<IReadOnlyCollection<Person>> ListLicenced(CancellationToken token)
{
    var sql = "select data from persons where (data ->> 'HasLicence')::bool = true";
    var results = await _connection.QueryAsync<string>(sql);
    return results.Select(x => _json.FromJson<Person>(x)).ToList();
}
```


### index usage
Examine the index being used by running `explain analyze ...`

```sql
-- query using idx_car_owner_id index
explain analyze select data from cars where (data -> 'Owner' ->> 'Id')::uuid= 'f7c8bc0c-87d7-46d4-86f6-37db01e27ee3'::uuid;

Bitmap Heap Scan on cars  (cost=4.18..12.68 rows=4 width=32) (actual time=0.008..0.010 rows=0 loops=1)
  Recheck Cond: ((((data -> 'Owner'::text) ->> 'Id'::text))::uuid = 'f7c8bc0c-87d7-46d4-86f6-37db01e27ee3'::uuid)
  ->  Bitmap Index Scan on idx_car_owner_id  (cost=0.00..4.18 rows=4 width=0) (actual time=0.004..0.005 rows=0 loops=1)
        Index Cond: ((((data -> 'Owner'::text) ->> 'Id'::text))::uuid = 'f7c8bc0c-87d7-46d4-86f6-37db01e27ee3'::uuid)
Planning Time: 0.127 ms
Execution Time: 0.030 ms


-- query using idx_person_has_licence index
explain analyze select data from persons where (data ->> 'HasLicence')::bool = true;

Bitmap Heap Scan on persons  (cost=8.30..27.66 rows=535 width=32) (actual time=0.015..0.017 rows=1 loops=1)
  Filter: ((data ->> 'HasLicence'::text))::boolean
  Heap Blocks: exact=1
  ->  Bitmap Index Scan on idx_person_has_licence  (cost=0.00..8.16 rows=535 width=0) (actual time=0.007..0.008 rows=1 loops=1)
        Index Cond: (((data ->> 'HasLicence'::text))::boolean = true)
Planning Time: 0.049 ms
Execution Time: 0.035 ms
```

## other queries
```sql
select * from cars;
select data -> 'Owner' as owner_as_json from cars;
select data ->> 'Owner' as owner_as_string from cars;
select data -> 'Owner' ->> 'Id' as id from cars;
select (data -> 'Owner' ->> 'Id')::uuid as guid from cars;
```
