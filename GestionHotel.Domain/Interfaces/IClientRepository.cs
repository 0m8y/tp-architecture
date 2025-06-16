using GestionHotel.Domain.Entities;

namespace GestionHotel.Domain.Interfaces;

public interface IClientRepository
{
    Client? GetById(Guid id);
    void Add(Client client);
    bool ExistsByEmail(string email);
    Client? GetByEmail(string email);
}
