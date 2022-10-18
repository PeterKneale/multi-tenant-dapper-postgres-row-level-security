using Demo.Domain.PersonAggregate;
using FluentValidation;
using MediatR;

namespace Demo.Application.Queries;

public static class ListUnlicencedPerson
{
    public record Query : IRequest<IReadOnlyCollection<Result>>;

    public record Result(Guid Id, string Name);

    internal class Validator : AbstractValidator<Query>
    {
    }

    internal class Handler : IRequestHandler<Query, IReadOnlyCollection<Result>>
    {
        private readonly IPersonRepository _persons;

        public Handler(IPersonRepository persons)
        {
            _persons = persons;
        }

        public async Task<IReadOnlyCollection<Result>> Handle(Query request, CancellationToken cancellationToken)
        {
            var persons = await _persons.ListUnlicenced(cancellationToken);
            return persons
                .Select(x => new Result(x.Id.Id, x.Name.FullName))
                .ToList();
        }
    }
}