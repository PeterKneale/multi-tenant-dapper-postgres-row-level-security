namespace Demo.Domain.PersonAggregate;

public class Licence
{
    private Licence(string licenceNumber)
    {
        LicenceNumber = licenceNumber;
    }

    private Licence()
    {
        // Parameterless constructor for serialisation
    }

    // Private setter for serialisation
    public string LicenceNumber { get; private set; }

    public static Licence CreateInstance(string licenceNumber)
    {
        return new Licence(licenceNumber);
    }
}