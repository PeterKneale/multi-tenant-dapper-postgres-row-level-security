using Demo.Domain.PersonAggregate;
using FluentValidation;
using MediatR;

namespace Demo.Application.Commands;

public static class AddPerson
{
    public class Command : IRequest
    {
        public Command(Guid id, string firstName, string lastName)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
        }

        public Guid Id { get; }
        public string FirstName { get; }
        public string LastName { get; }
    }

    internal class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
        }
    }

    internal class Handler : IRequestHandler<Command>
    {
        private readonly IPersonRepository _persons;

        public Handler(IPersonRepository persons)
        {
            _persons = persons;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var id = PersonId.CreateInstance(request.Id);
            var name = Name.CreateInstance(request.FirstName, request.LastName);

            var person = Person.CreateInstance(id, name);

            await _persons.SavePerson(person, cancellationToken);

            return Unit.Value;
        }
    }
}