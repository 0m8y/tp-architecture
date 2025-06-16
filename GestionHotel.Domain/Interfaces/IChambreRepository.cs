using GestionHotel.Domain.Entities;

namespace GestionHotel.Domain.Interfaces;

public interface IChambreRepository
{
    List<Chambre> GetAvailableRooms(DateTime from, DateTime to);
    Chambre? GetById(Guid id);
    List<Chambre> GetAll();
}
