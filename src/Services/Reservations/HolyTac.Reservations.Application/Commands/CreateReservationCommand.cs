using MediatR;

namespace HolyTac.Reservations.Application.Commands;

public record CreateReservationCommand(
    string CustomerName,
    string Phone,
    string? Email,
    int PartySize,
    DateTime ReservationAtUtc,
    string? Notes) : IRequest<Guid>;

public class CreateReservationCommandHandler(IReservationRepository reservationRepository)
    : IRequestHandler<CreateReservationCommand, Guid>
{
    public async Task<Guid> Handle(CreateReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await reservationRepository.CreateAsync(
            request.CustomerName,
            request.Phone,
            request.Email,
            request.PartySize,
            request.ReservationAtUtc,
            request.Notes,
            cancellationToken);

        return reservation.Id;
    }
}
