using HolyTac.Reservations.Domain;

namespace HolyTac.Reservations.Application;

public record ReservationDto(
    Guid Id,
    string CustomerName,
    string Phone,
    string? Email,
    int PartySize,
    DateTime ReservationAtUtc,
    int TableNumber,
    string Status,
    string? Notes,
    DateTime CreatedAtUtc)
{
    public static ReservationDto FromDomain(Reservation r) => new(
        r.Id, r.CustomerName, r.Phone, r.Email, r.PartySize, r.ReservationAtUtc,
        r.TableNumber, r.Status.ToString(), r.Notes, r.CreatedAtUtc);
}
