namespace Demo.Application.Commands;

public static class RegisterCar
{
    public class Command : IRequest
    {
        public Command(Guid id, string registration)
        {
            Id = id;
            Registration = registration;
        }

        public Guid Id { get; }
        public string Registration { get; }
    }

    internal class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Registration).NotEmpty();
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
            var carId = CarId.CreateInstance(request.Id);
            var registration = Registration.CreateInstance(request.Registration);

            var car = await _cars.Get(carId, cancellationToken);
            if (car == null)
            {
                throw new Exception("Car not found");
            }

            car.Register(registration);

            await _cars.Update(car, cancellationToken);

            return Unit.Value;
        }
    }
}