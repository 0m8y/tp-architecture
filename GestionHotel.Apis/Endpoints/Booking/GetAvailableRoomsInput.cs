namespace GestionHotel.Apis.Endpoints.Booking;

public class GetAvailableRoomsInput
{
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
}