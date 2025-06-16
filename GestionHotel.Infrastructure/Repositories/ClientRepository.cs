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

    public bool ExistsByEmail(string email)
    {
        return _context.Clients.Any(c => c.Email == email);
    }

    public Client? GetByEmail(string email)
    {
        return _context.Clients.FirstOrDefault(c => c.Email == email);
    }

}
