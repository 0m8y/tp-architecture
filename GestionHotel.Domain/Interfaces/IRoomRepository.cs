using GestionHotel.Domain.Entities;

namespace GestionHotel.Domain.Interfaces;

public interface IRoomRepository
{
    List<Room> GetAvailableRooms(DateTime from, DateTime to, int minCapacity);
    Room? GetById(Guid id);
    List<Room> GetAll();
}
