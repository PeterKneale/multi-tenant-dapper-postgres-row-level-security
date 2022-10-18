using Demo.Domain.CarAggregate;
using Demo.Domain.PersonAggregate;
using FluentValidation;
using MediatR;

namespace Demo.Application.Queries;

public static class ListByOwner
{
    public record Query(Guid UserId) : IRequest<IReadOnlyCollection<Result>>;

    public record Result(Guid Id, Guid OwnerId, string? Registration);

    internal class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
        }
    }

    internal class Handler : IRequestHandler<Query, IReadOnlyCollection<Result>>
    {
        private readonly ICarRepository _cars;

        public Handler(ICarRepository cars)
        {
            _cars = cars;
        }

        public async Task<IReadOnlyCollection<Result>> Handle(Query request, CancellationToken cancellationToken)
        {
            var ownerId = PersonId.CreateInstance(request.UserId);
            
            var persons = await _cars.ListByOwner(ownerId, cancellationToken);
            
            return persons
                .Select(car =>  new Result(car.Id.Id, car.Owner!.Id, car.Registration?.RegistrationNumber))
                .ToList();
        }
    }
}