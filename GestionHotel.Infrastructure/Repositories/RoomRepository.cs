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

    public List<Room> GetAvailableRooms(DateTime from, DateTime to)
    {
        var reservedRoomIds = _context.ReservationRooms
            .Where(rr =>
                (from < rr.Reservation.EndDate && to > rr.Reservation.StartDate))
            .Select(rr => rr.RoomId)
            .Distinct();

        return _context.Rooms
            .Where(r => !reservedRoomIds.Contains(r.Id))
            .ToList();
    }


    public Room? GetById(Guid id) => _context.Rooms.Find(id);

    public List<Room> GetAll() => _context.Rooms.ToList();
}
