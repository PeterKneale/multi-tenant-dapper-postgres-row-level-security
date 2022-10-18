using Demo.Domain.PersonAggregate;
using FluentValidation;
using MediatR;

namespace Demo.Application.Queries;

public static class GetPerson
{
    public class Query : IRequest<Result>
    {
        public Query(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }
    }

    public record Result(Guid Id, string Name);

    internal class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    internal class Handler : IRequestHandler<Query, Result>
    {
        private readonly IPersonRepository _persons;

        public Handler(IPersonRepository persons)
        {
            _persons = persons;
        }

        public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
        {
            var personId = PersonId.CreateInstance(request.Id);

            var person = await _persons.GetPerson(personId, cancellationToken);
            if (person == null) throw new Exception("Person not found");

            return new Result(person.Id.Id, person.Name.FullName);
        }
    }
}