using GestionHotel.Domain.Entities;
using GestionHotel.Domain.Interfaces;
using GestionHotel.Infrastructure.Data;

namespace GestionHotel.Infrastructure.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly HotelDbContext _context;

    public ClientRepository(HotelDbContext context)
    {
        _context = context;
    }

    public void Add(Client client)
    {
        _context.Clients.Add(client);
        _context.SaveChanges();
    }

    public Client? GetById(Guid id)
    {
        return _context.Clients.Find(id);
    }
}
