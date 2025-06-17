using GestionHotel.Domain.Entities;
using GestionHotel.Domain.Interfaces;

namespace GestionHotel.Application.UseCases.Booking;

public class GetAvailableRooms
{
    private readonly IRoomRepository _roomRepository;

    public GetAvailableRooms(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    public IEnumerable<Room> Execute(DateTime startDate, DateTime endDate)
    {
        return _roomRepository.GetAvailableRooms(startDate, endDate);
    }
}
