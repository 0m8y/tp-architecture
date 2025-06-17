using GestionHotel.Domain.Enums;

public class PayReservationRequest
{
    public string CardNumber { get; set; } = "";
    public string ExpiryDate { get; set; } = "";
    public PaymentProvider Provider { get; set; }
}
