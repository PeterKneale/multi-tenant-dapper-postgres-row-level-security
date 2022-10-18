namespace Demo.Domain.PersonAggregate;

public class Name
{
    private Name(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    private Name()
    {
        // Parameterless constructor for serialisation
    }

    // Private setter for serialisation
    public string FirstName { get; private set; }

    // Private setter for serialisation
    public string LastName { get; private set; }

    public string FullName => $"{FirstName} {LastName}";

    public static Name CreateInstance(string firstName, string lastName)
    {
        return new Name(firstName, lastName);
    }
}