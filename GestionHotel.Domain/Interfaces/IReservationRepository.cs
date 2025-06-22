using GestionHotel.Domain.Entities;

namespace GestionHotel.Domain.Interfaces;

public interface IReservationRepository
{
    Reservation? GetById(Guid id);
    void Create(Reservation reservation);
    void Cancel(Guid reservationId);
    void Update(Reservation reservation);
    List<Reservation> GetByClientId(Guid clientId);
    Reservation? GetByReservationId(Guid reservationId);
    List<Reservation> GetAll();
}
