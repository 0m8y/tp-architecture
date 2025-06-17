namespace GestionHotel.Domain.Entities;

public class ReservationRoom
{
    public Guid ReservationId { get; set; }
    public Reservation Reservation { get; set; } = null!;

    public Guid RoomId { get; set; }
    public Room Room { get; set; } = null!;
}
