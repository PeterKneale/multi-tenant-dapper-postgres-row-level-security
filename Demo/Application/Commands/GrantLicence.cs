using Demo.Domain.PersonAggregate;
using FluentValidation;
using MediatR;

namespace Demo.Application.Commands;

public static class GrantLicence
{
    public class Command : IRequest
    {
        public Command(Guid personId, string licenceNumber)
        {
            PersonId = personId;
            LicenceNumber = licenceNumber;
        }

        public Guid PersonId { get; }
        public string LicenceNumber { get; }
    }

    internal class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.PersonId).NotEmpty();
            RuleFor(x => x.LicenceNumber).NotEmpty();
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
            var personId = PersonId.CreateInstance(request.PersonId);
            var licence = Licence.CreateInstance(request.LicenceNumber);

            var person = await _persons.GetPerson(personId, cancellationToken);
            if (person == null) throw new Exception("Person not found");

            person.GrantLicence(licence);

            await _persons.UpdatePerson(person, cancellationToken);

            return Unit.Value;
        }
    }
}