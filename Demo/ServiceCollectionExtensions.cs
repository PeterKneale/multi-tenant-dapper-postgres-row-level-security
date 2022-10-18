using System.Data;
using System.Reflection;
using Demo.Domain.CarAggregate;
using Demo.Domain.PersonAggregate;
using Demo.Infrastructure.Database;
using Demo.Infrastructure.Repositories;
using FluentMigrator.Runner;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Demo;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDemo(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration["ConnectionString"];

        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(assembly);
        services.AddValidatorsFromAssembly(assembly);

        services.AddScoped<ICarRepository, CarRepository>();
        services.AddScoped<IPersonRepository, PersonRepository>();
        services.AddScoped<IDbConnection>(c => new NpgsqlConnection(connectionString));

        services
            .AddFluentMigratorCore()
            .ConfigureRunner(runner => runner
                .AddPostgres()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(Assembly.GetExecutingAssembly()).For.Migrations());
        services
            .AddScoped<MigrationExecutor>();

        services
            .AddSingleton<IJsonSerializer, JsonSerializer>();

        return services;
    }
}