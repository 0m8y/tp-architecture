namespace GestionHotel.Apis.DTOs;

public class CreateReservationRequest
{
    public List<Guid> RoomIds { get; set; } = new();
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
