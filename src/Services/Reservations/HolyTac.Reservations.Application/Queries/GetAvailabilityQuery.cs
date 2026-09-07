using MediatR;

namespace HolyTac.Reservations.Application.Queries;

public record GetAvailabilityQuery(DateOnly Date, int PartySize) : IRequest<IReadOnlyList<TimeSpan>>;

public class GetAvailabilityQueryHandler(AvailabilityService availabilityService)
    : IRequestHandler<GetAvailabilityQuery, IReadOnlyList<TimeSpan>>
{
    public Task<IReadOnlyList<TimeSpan>> Handle(GetAvailabilityQuery request, CancellationToken cancellationToken)
        => availabilityService.GetAvailableSlotsAsync(request.Date, request.PartySize, cancellationToken);
}
