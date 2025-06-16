namespace GestionHotel.Domain.Entities;

public class Paiement
{
    public Guid Id { get; set; }
    public Guid ReservationId { get; set; }
    public decimal Montant { get; set; }
    public DateTime DatePaiement { get; set; }
    public bool EstRembourse { get; set; }
}
