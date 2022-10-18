using Demo.Domain.PersonAggregate;

namespace Demo.Domain.CarAggregate;

public interface ICarRepository
{
    Task<Car?> GetCar(CarId id, CancellationToken cancellationToken);

    Task<Car?> GetByRegistration(Registration registration, CancellationToken cancellationToken);

    Task SaveCar(Car car, CancellationToken cancellationToken);

    Task UpdateCar(Car car, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Car>> ListByOwner(PersonId ownerId, CancellationToken cancellationToken);
}