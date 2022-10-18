namespace Demo.Domain.PersonAggregate;

public class PersonId
{
    private PersonId(Guid id)
    {
        Id = id;
    }

    private PersonId()
    {
        // Parameterless constructor for serialisation
    }

    // Private setter for serialisation
    public Guid Id { get; private set; }

    public static PersonId CreateInstance(Guid? id = null)
    {
        return new PersonId(id ?? Guid.NewGuid());
    }
}