using System.Collections.Concurrent;
using GestionHotel.Domain.Entities;
using GestionHotel.Domain.Interfaces;

namespace GestionHotel.Infrastructure.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly ConcurrentDictionary<Guid, Payment> _payments = new();

    public void Save(Payment payment)
    {
        _payments[payment.ReservationId] = payment;
    }

    public Payment? GetByReservationId(Guid reservationId)
    {
        _payments.TryGetValue(reservationId, out var payment);
        return payment;
    }
}
