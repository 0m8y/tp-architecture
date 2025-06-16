namespace GestionHotel.Domain.Interfaces;

public interface IPaiementRepository
{
    void EnregistrerPaiement(Paiement paiement);
    Paiement? GetByReservationId(Guid reservationId);
}
