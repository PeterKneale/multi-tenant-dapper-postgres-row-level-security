using Demo.Domain.CarAggregate;
using Demo.Domain.PersonAggregate;
using FluentValidation;
using MediatR;

namespace Demo.Application.Commands;

public static class TakeOwnership
{
    public class Command : IRequest
    {
        public Command(Guid personId, Guid carId)
        {
            PersonId = personId;
            CarId = carId;
        }

        public Guid PersonId { get; }
        public Guid CarId { get; }
    }

    internal class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.PersonId).NotEmpty();
            RuleFor(x => x.CarId).NotEmpty();
        }
    }

    internal class Handler : IRequestHandler<Command>
    {
        private readonly ICarRepository _cars;

        public Handler(ICarRepository cars)
        {
            _cars = cars;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var carId = CarId.CreateInstance(request.CarId);
            var ownerId = PersonId.CreateInstance(request.PersonId);

            var car = await _cars.GetCar(carId, cancellationToken);
            if (car == null) throw new Exception("Car not found");

            car.ChangeOwner(ownerId);

            await _cars.UpdateCar(car, cancellationToken);

            return Unit.Value;
        }
    }
}