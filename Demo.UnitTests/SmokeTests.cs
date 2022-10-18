using Demo.Domain.CarAggregate;
using Demo.Domain.PersonAggregate;

namespace Demo.UnitTests;

public class SmokeTests
{
    [Fact]
    public void SmokeTest()
    {
        var name = Name.CreateInstance("John", "Smith");
        var personId = PersonId.CreateInstance();
        var person = Person.CreateInstance(personId, name);
        var licence = Licence.CreateInstance("123456789");
        person.GrantLicence(licence);

        var carId = CarId.CreateInstance();
        var car = Car.CreateInstance(carId);
        car.ChangeOwner(person.Id);
        var registration = Registration.CreateInstance("ACB-123");
        car.Register(registration);
    }
}