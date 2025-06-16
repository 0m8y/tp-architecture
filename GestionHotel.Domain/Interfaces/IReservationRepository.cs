using GestionHotel.Domain.Entities;

namespace GestionHotel.Domain.Interfaces;

public interface IReservationRepository
{
    Reservation? GetById(Guid id);
    void Create(Reservation reservation);
    void Cancel(Guid reservationId);
    List<Reservation> GetByClientId(Guid clientId);
}
