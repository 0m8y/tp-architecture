using GestionHotel.Domain.Entities;

namespace GestionHotel.Domain.Interfaces;

public interface IPaymentRepository
{
    void Save(Payment payment);
    Payment? GetByReservationId(Guid reservationId);
}
