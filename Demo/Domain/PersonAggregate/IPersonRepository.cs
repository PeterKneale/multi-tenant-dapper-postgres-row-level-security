namespace Demo.Domain.PersonAggregate;

public interface IPersonRepository
{
    Task<Person?> GetPerson(PersonId personId, CancellationToken cancellationToken);

    Task SavePerson(Person person, CancellationToken cancellationToken);

    Task UpdatePerson(Person person, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Person>> ListLicenced(CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Person>> ListUnlicenced(CancellationToken cancellationToken);
}