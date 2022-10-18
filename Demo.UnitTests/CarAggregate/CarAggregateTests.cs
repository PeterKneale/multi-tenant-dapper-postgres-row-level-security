using Demo.Domain.CarAggregate;
using Demo.Domain.PersonAggregate;

namespace Demo.UnitTests.CarAggregate;

public class CarAggregateTests
{
    [Fact]
    public void CarsHaveNoInitialOwner()
    {
        var car = Car.CreateInstance(CarId.CreateInstance());
        car.Owner.Should().BeNull();
    }

    [Fact]
    public void CarOwnershipCanChange()
    {
        var carId = CarId.CreateInstance();
        var car = Car.CreateInstance(carId);
        var name = Name.CreateInstance("John", "Smith");
        var personId = PersonId.CreateInstance();
        var person = Person.CreateInstance(personId, name);
        car.ChangeOwner(person.Id);

        car.Owner.Should().BeEquivalentTo(person.Id);
    }

    [Fact]
    public void CarCanBeRegisteredIfOwned()
    {
        var carId = CarId.CreateInstance();
        var car = Car.CreateInstance(carId);
        var name = Name.CreateInstance("John", "Smith");
        var personId = PersonId.CreateInstance();
        var person = Person.CreateInstance(personId, name);
        car.ChangeOwner(person.Id);
        var registration = Registration.CreateInstance("ACB-123");
        car.Register(registration);

        car.Registration.Should().BeEquivalentTo(registration);
    }
}