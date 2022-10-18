namespace Demo.Domain.PersonAggregate;

public class Person
{
    private Person(PersonId id, Name name, Licence? licence)
    {
        Id = id;
        Name = name;
        Licence = licence;
    }

    private Person()
    {
        // Parameterless constructor for serialisation
    }

    // Private setter for serialisation
    public PersonId Id { get; private set; }

    // Private setter for serialisation
    public Name Name { get; private set; }

    public Licence? Licence { get; private set; }

    public bool HasLicence => Licence != null;

    public void GrantLicence(Licence licence)
    {
        Licence = licence;
    }

    public static Person CreateInstance(PersonId id, Name name, Licence? licence = null)
    {
        return new Person(id, name, licence);
    }
}