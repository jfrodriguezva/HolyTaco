using MediatR;

namespace HolyTac.Reservations.Application.Queries;

public record GetReservationByIdQuery(Guid Id) : IRequest<ReservationDto?>;

public class GetReservationByIdQueryHandler(IReservationRepository repository)
    : IRequestHandler<GetReservationByIdQuery, ReservationDto?>
{
    public async Task<ReservationDto?> Handle(GetReservationByIdQuery request, CancellationToken cancellationToken)
    {
        var reservation = await repository.GetByIdAsync(request.Id, cancellationToken);
        return reservation is null ? null : ReservationDto.FromDomain(reservation);
    }
}
