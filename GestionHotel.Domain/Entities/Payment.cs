namespace GestionHotel.Domain.Entities;

public class Payment
{
    public Guid Id { get; set; }
    public Guid ReservationId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public bool IsRefunded { get; set; }
}
