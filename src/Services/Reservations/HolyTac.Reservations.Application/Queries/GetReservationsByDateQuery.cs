using MediatR;

namespace HolyTac.Reservations.Application.Queries;

public record GetReservationsByDateQuery(DateOnly Date) : IRequest<IReadOnlyList<ReservationDto>>;

public class GetReservationsByDateQueryHandler(IReservationRepository repository)
    : IRequestHandler<GetReservationsByDateQuery, IReadOnlyList<ReservationDto>>
{
    public async Task<IReadOnlyList<ReservationDto>> Handle(GetReservationsByDateQuery request, CancellationToken cancellationToken)
    {
        var reservations = await repository.GetByDateAsync(request.Date, cancellationToken);
        return reservations
            .OrderBy(r => r.ReservationAtUtc)
            .Select(ReservationDto.FromDomain)
            .ToList();
    }
}
