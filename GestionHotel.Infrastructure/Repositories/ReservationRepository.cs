using GestionHotel.Domain.Entities;
using GestionHotel.Domain.Interfaces;
using GestionHotel.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestionHotel.Infrastructure.Repositories;

public class ReservationRepository : IReservationRepository
{
    private readonly HotelDbContext _context;

    public ReservationRepository(HotelDbContext context)
    {
        _context = context;
    }

    public Reservation? GetById(Guid id)
    {
        return _context.Reservations
            .Include(r => r.ReservationRooms)
                .ThenInclude(rr => rr.Room)
            .FirstOrDefault(r => r.Id == id);
    }

    public void Create(Reservation reservation)
    {
        _context.Reservations.Add(reservation);
        _context.SaveChanges();
    }

    public void Cancel(Guid reservationId)
    {
        var reservation = _context.Reservations.Find(reservationId);
        if (reservation == null) return;

        reservation.Status = Domain.Enums.ReservationStatus.Cancelled;
        _context.SaveChanges();
    }

    public void Update(Reservation reservation)
    {
        _context.Reservations.Update(reservation);
        _context.SaveChanges();
    }

    public List<Reservation> GetByClientId(Guid clientId)
    {
        return _context.Reservations
            .Include(r => r.ReservationRooms)
                .ThenInclude(rr => rr.Room)
            .Where(r => r.ClientId == clientId)
            .ToList();
    }

    public List<Reservation> GetAll()
    {
        return _context.Reservations
            .Include(r => r.ReservationRooms)
                .ThenInclude(rr => rr.Room)
            .ToList();
    }
}
