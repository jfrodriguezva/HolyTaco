using HolyTac.Reservations.Domain;

namespace HolyTac.Reservations.Infrastructure;

internal class TableRow
{
    public Guid Id { get; set; }
    public int Number { get; set; }
    public int Capacity { get; set; }
    public int Zone { get; set; }

    public Table ToDomain() => Table.Load(Id, Number, Capacity, (Zone)Zone);
}

internal class ReservationRow
{
    public Guid Id { get; set; }
    public string CustomerName { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public string? Email { get; set; }
    public int PartySize { get; set; }
    public DateTime ReservationAtUtc { get; set; }
    public int TableNumber { get; set; }
    public int Status { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public Reservation ToDomain() => Reservation.Load(
        Id, CustomerName, Phone, Email, PartySize, ReservationAtUtc, TableNumber,
        (ReservationStatus)Status, Notes, CreatedAtUtc);
}
