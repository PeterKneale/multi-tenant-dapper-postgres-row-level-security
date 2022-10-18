using System;
using System.Threading.Tasks;
using Demo.Application.Commands;
using Demo.Application.Queries;
using Demo.IntegrationTests.Fixtures;
using FluentAssertions;
using Xunit;

namespace Demo.IntegrationTests.Application.Commands;

[Collection(nameof(ContainerCollectionFixture))]
public class AddPersonTests
{
    private readonly IServiceProvider _provider;

    public AddPersonTests(ContainerFixture container)
    {
        _provider = container.Provider;
    }

    [Fact]
    public async Task CanAddPerson()
    {
        // arrange
        var id = Guid.NewGuid();
        var firstName = "john";
        var lastName = "smith";

        // act
        await _provider.ExecuteCommand(new AddPerson.Command(id, firstName, lastName));

        // assert
        var result = await _provider.ExecuteQuery(new GetPerson.Query(id));
        result.Id.Should().Be(id);
        result.Name.Should().Be("john smith");
    }
}