using GestionHotel.Domain.Entities;
using GestionHotel.Domain.Interfaces;
using GestionHotel.Infrastructure.Data;

public class RoomRepository : IRoomRepository
{
    private readonly HotelDbContext _context;

    public RoomRepository(HotelDbContext context)
    {
        _context = context;
    }

    public List<Room> GetAvailableRooms(DateTime from, DateTime to, int minCapacity)
    {
        // Logique de filtrage de base
        return _context.Rooms
            .Where(r => r.Capacity >= minCapacity &&
                        !_context.Reservations.Any(res =>
                            res.RoomId == r.Id &&
                            ((from >= res.StartDate && from < res.EndDate) ||
                             (to > res.StartDate && to <= res.EndDate))))
            .ToList();
    }

    public Room? GetById(Guid id) => _context.Rooms.Find(id);

    public List<Room> GetAll() => _context.Rooms.ToList();
}
