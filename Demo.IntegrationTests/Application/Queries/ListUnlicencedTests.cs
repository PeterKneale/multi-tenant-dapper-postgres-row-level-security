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
public class ListUnlicencedTests
{
    private readonly IServiceProvider _provider;

    public ListUnlicencedTests(ContainerFixture container)
    {
        _provider = container.Provider;
    }

    [Fact]
    public async Task UnlicencedPersonReturned()
    {
        // arrange
        var id = Guid.NewGuid();
        var firstName = "john";
        var lastName = "smith";

        // act
        await _provider.ExecuteCommand(new AddPerson.Command(id, firstName, lastName));

        // assert
        var result1 = await _provider.ExecuteQuery(new ListUnlicencedPerson.Query());
        result1.Select(x => x.Id).Should().Contain(id);
        var result2 = await _provider.ExecuteQuery(new ListLicencedPerson.Query());
        result2.Select(x => x.Id).Should().NotContain(id);
    }
}