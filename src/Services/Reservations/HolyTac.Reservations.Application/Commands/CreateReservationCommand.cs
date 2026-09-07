using HolyTac.Reservations.Domain;
using MediatR;

namespace HolyTac.Reservations.Application.Commands;

public record CreateReservationCommand(
    string CustomerName,
    string Phone,
    string? Email,
    int PartySize,
    DateTime ReservationAtUtc,
    string? Notes) : IRequest<Guid>;

public class CreateReservationCommandHandler(
    AvailabilityService availabilityService,
    IReservationRepository reservationRepository) : IRequestHandler<CreateReservationCommand, Guid>
{
    public async Task<Guid> Handle(CreateReservationCommand request, CancellationToken cancellationToken)
    {
        var table = await availabilityService.FindAvailableTableAsync(
            request.ReservationAtUtc, request.PartySize, cancellationToken)
            ?? throw new InvalidOperationException("No hay mesas disponibles para ese horario y tamaño de grupo.");

        var reservation = Reservation.Create(
            request.CustomerName,
            request.Phone,
            request.Email,
            request.PartySize,
            request.ReservationAtUtc,
            table.Number,
            request.Notes);

        await reservationRepository.AddAsync(reservation, cancellationToken);
        return reservation.Id;
    }
}
