using Demo.Domain.PersonAggregate;

namespace Demo.UnitTests.PersonAggregate;

public class PersonAggregateTests
{
    [Fact]
    public void NamesHaveFullNames()
    {
        var name = Name.CreateInstance("John", "Smith");
        name.FullName.Should().Be("John Smith");
    }

    [Fact]
    public void PeopleHaveNoInitialLicence()
    {
        var name = Name.CreateInstance("John", "Smith");
        var id = PersonId.CreateInstance();
        var person = Person.CreateInstance(id, name);
        person.HasLicence.Should().BeFalse();
    }

    [Fact]
    public void PeopleCanHaveALicenceGranted()
    {
        var name = Name.CreateInstance("John", "Smith");
        var id = PersonId.CreateInstance();
        var person = Person.CreateInstance(id, name);
        var licence = Licence.CreateInstance("123456789");
        person.GrantLicence(licence);
        person.Licence.Should().BeEquivalentTo(licence);
    }
}