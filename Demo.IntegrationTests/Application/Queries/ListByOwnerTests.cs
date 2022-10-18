using System;
using System.Linq;
using System.Threading.Tasks;
using Demo.Application.Commands;
using Demo.Application.Queries;
using Demo.IntegrationTests.Fixtures;
using FluentAssertions;
using Xunit;

namespace Demo.IntegrationTests.Application.Queries;

[Collection(nameof(ContainerCollectionFixture))]
public class ListByOwnerTests
{
    private readonly IServiceProvider _provider;

    public ListByOwnerTests(ContainerFixture container)
    {
        _provider = container.Provider;
    }

    [Fact]
    public async Task CarsReturnedByOwner()
    {
        // arrange
        var carId = Guid.NewGuid();
        var personId = Guid.NewGuid();
        var firstName = "john";
        var lastName = "smith";

        // act
        await _provider.ExecuteCommand(new AddCar.Command(carId));
        await _provider.ExecuteCommand(new AddPerson.Command(personId, firstName, lastName));
        await _provider.ExecuteCommand(new TakeOwnership.Command(personId, carId));

        // assert
        var results = await _provider.ExecuteQuery(new ListByOwner.Query(personId));
        results.Select(x => x.Id).Should().Contain(carId);
    }
}