using GestionHotel.Domain.Enums;

namespace GestionHotel.Domain.Entities;

public class Reservation
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public List<Guid> ChambreIds { get; set; } = new();
    public DateTime DateDebut { get; set; }
    public DateTime DateFin { get; set; }
    public decimal Total { get; set; }
    public bool EstPayee { get; set; }
    public StatutReservation Status { get; set; }
}
