using MediatR;

namespace HolyTac.Reservations.Application.Commands;

public record CancelReservationCommand(Guid ReservationId) : IRequest;

public class CancelReservationCommandHandler(IReservationRepository repository) : IRequestHandler<CancelReservationCommand>
{
    public async Task Handle(CancelReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await repository.GetByIdAsync(request.ReservationId, cancellationToken)
            ?? throw new InvalidOperationException($"No se encontró la reservación {request.ReservationId}.");

        reservation.Cancel();
        await repository.UpdateStatusAsync(reservation, cancellationToken);
    }
}
