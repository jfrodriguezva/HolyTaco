using MediatR;

namespace HolyTac.Reservations.Application.Commands;

public record ConfirmReservationCommand(Guid ReservationId) : IRequest;

public class ConfirmReservationCommandHandler(IReservationRepository repository) : IRequestHandler<ConfirmReservationCommand>
{
    public async Task Handle(ConfirmReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await repository.GetByIdAsync(request.ReservationId, cancellationToken)
            ?? throw new InvalidOperationException($"No se encontró la reservación {request.ReservationId}.");

        reservation.Confirm();
        await repository.UpdateStatusAsync(reservation, cancellationToken);
    }
}
