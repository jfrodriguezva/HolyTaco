using HolyTac.SharedKernel;

namespace HolyTac.Reservations.Domain;

public class Table : Entity<Guid>
{
    public int Number { get; private set; }
    public int Capacity { get; private set; }
    public Zone Zone { get; private set; }

    private Table() { }

    public static Table Load(Guid id, int number, int capacity, Zone zone) => new()
    {
        Id = id,
        Number = number,
        Capacity = capacity,
        Zone = zone
    };
}
