using System;
using System.Threading.Tasks;
using Demo.Application.Commands;
using Demo.Application.Queries;
using Demo.IntegrationTests.Fixtures;
using FluentAssertions;
using Xunit;

namespace Demo.IntegrationTests.Application.Commands;

[Collection(nameof(ContainerCollectionFixture))]
public class RegisterCarTests
{
    private readonly IServiceProvider _provider;

    public RegisterCarTests(ContainerFixture container)
    {
        _provider = container.Provider;
    }

    [Fact]
    public async Task CanRegisterCar()
    {
        // arrange
        var id = Guid.NewGuid();
        var registration = "abc";

        // act
        await _provider.ExecuteCommand(new AddCar.Command(id));
        await _provider.ExecuteCommand(new RegisterCar.Command(id, registration));

        // assert
        var result = await _provider.ExecuteQuery(new GetCarByRegistration.Query(registration));
        result.Id.Should().Be(id);
        result.Registration.Should().Be(registration);
    }
}