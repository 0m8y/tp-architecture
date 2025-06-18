using GestionHotel.Domain.Entities;

namespace GestionHotel.Domain.Interfaces;

public interface IPaymentRepository
{
    void Add(Payment payment);
    void Update(Payment payment);
    Payment? GetByReservationId(Guid reservationId);
}
