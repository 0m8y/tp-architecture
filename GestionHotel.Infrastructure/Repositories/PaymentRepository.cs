using GestionHotel.Domain.Entities;
using GestionHotel.Domain.Interfaces;
using GestionHotel.Infrastructure.Data;

namespace GestionHotel.Infrastructure.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly HotelDbContext _context;

    public PaymentRepository(HotelDbContext context)
    {
        _context = context;
    }

    public void Add(Payment payment)
    {
        _context.Payments.Add(payment);
        _context.SaveChanges();
    }

    public void Update(Payment payment)
    {
        _context.Payments.Update(payment);
        _context.SaveChanges();
    }

    public Payment? GetByReservationId(Guid reservationId)
    {
        return _context.Payments.FirstOrDefault(p => p.ReservationId == reservationId);
    }
}
