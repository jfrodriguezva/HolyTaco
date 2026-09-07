using HolyTac.SharedKernel;

namespace HolyTac.Reservations.Domain;

public class Reservation : Entity<Guid>
{
    public string CustomerName { get; private set; } = default!;
    public string Phone { get; private set; } = default!;
    public string? Email { get; private set; }
    public int PartySize { get; private set; }
    public DateTime ReservationAtUtc { get; private set; }
    public int TableNumber { get; private set; }
    public ReservationStatus Status { get; private set; }
    public string? Notes { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public DateTime EndsAtUtc => ReservationAtUtc.AddMinutes(BusinessHours.ReservationDurationMinutes);
    public bool IsActive => Status is ReservationStatus.Pendiente or ReservationStatus.Confirmada;

    private Reservation() { }

    public static Reservation Create(
        string customerName,
        string phone,
        string? email,
        int partySize,
        DateTime reservationAtUtc,
        int tableNumber,
        string? notes)
    {
        if (string.IsNullOrWhiteSpace(customerName))
            throw new ArgumentException("El nombre del cliente es obligatorio.", nameof(customerName));

        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("El teléfono es obligatorio.", nameof(phone));

        if (partySize <= 0)
            throw new ArgumentException("El número de personas debe ser mayor a cero.", nameof(partySize));

        return new Reservation
        {
            Id = Guid.NewGuid(),
            CustomerName = customerName,
            Phone = phone,
            Email = email,
            PartySize = partySize,
            ReservationAtUtc = reservationAtUtc,
            TableNumber = tableNumber,
            Status = ReservationStatus.Pendiente,
            Notes = notes,
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    public static Reservation Load(
        Guid id, string customerName, string phone, string? email, int partySize,
        DateTime reservationAtUtc, int tableNumber, ReservationStatus status, string? notes, DateTime createdAtUtc)
        => new()
        {
            Id = id,
            CustomerName = customerName,
            Phone = phone,
            Email = email,
            PartySize = partySize,
            ReservationAtUtc = reservationAtUtc,
            TableNumber = tableNumber,
            Status = status,
            Notes = notes,
            CreatedAtUtc = createdAtUtc
        };

    /// <summary>True si esta reservación se traslapa en el tiempo con el rango [otroInicio, otroInicio + duración).</summary>
    public bool OverlapsWith(DateTime otherStartUtc)
    {
        var otherEnd = otherStartUtc.AddMinutes(BusinessHours.ReservationDurationMinutes);
        return ReservationAtUtc < otherEnd && otherStartUtc < EndsAtUtc;
    }

    public void Confirm()
    {
        if (Status != ReservationStatus.Pendiente)
            throw new InvalidOperationException($"No se puede confirmar una reservación en estado {Status}.");

        Status = ReservationStatus.Confirmada;
    }

    public void Cancel()
    {
        if (Status is ReservationStatus.Cancelada or ReservationStatus.Completada)
            throw new InvalidOperationException($"No se puede cancelar una reservación en estado {Status}.");

        Status = ReservationStatus.Cancelada;
    }
}
