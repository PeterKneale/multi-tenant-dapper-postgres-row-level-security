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
        var result = await _provider.ExecuteQuery(new GetCar.Query(id));

        // assert
        result.Id.Should().Be(id);
        result.Registration.Should().BeNull();
    }
}