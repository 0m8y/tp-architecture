using GestionHotel.Domain.Enums;

namespace GestionHotel.Domain.Entities;

public class Reservation
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public Guid RoomId { get; set; } = new();
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalAmount { get; set; }
    public bool IsPaid { get; set; }
    public ReservationStatus Status { get; set; }
}
