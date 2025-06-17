namespace GestionHotel.Apis.DTOs;

public class PayReservationRequest
{
    public string CardNumber { get; set; } = string.Empty;
    public string ExpiryDate { get; set; } = string.Empty;
}
