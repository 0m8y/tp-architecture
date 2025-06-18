namespace GestionHotel.Apis.DTOs;

public class RoomToCleanDto
{
    public Guid RoomId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public DateTime LastOccupied { get; set; }
    public DateTime? NextOccupied { get; set; }
}
