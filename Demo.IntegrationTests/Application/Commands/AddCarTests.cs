using System;
using System.Threading.Tasks;
using Demo.Application.Commands;
using Demo.Application.Queries;
using Demo.IntegrationTests.Fixtures;
using FluentAssertions;
using Xunit;

namespace Demo.IntegrationTests.Application.Commands;

[Collection(nameof(ContainerCollectionFixture))]
public class AddCarTests
{
    private readonly IServiceProvider _provider;

    public AddCarTests(ContainerFixture container)
    {
        _provider = container.Provider;
    }

    [Fact]
    public async Task CanAddCar()
    {
        // arrange
        var id = Guid.NewGuid();

        // act
        await _provider.ExecuteCommand(new AddCar.Command(id));

        // assert
        var result = await _provider.ExecuteQuery(new GetCar.Query(id));
        result.Id.Should().Be(id);
        result.Registration.Should().BeNull();
        result.OwnerId.Should().BeNull();
    }
}