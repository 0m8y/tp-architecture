using Microsoft.EntityFrameworkCore;
using GestionHotel.Domain.Entities;

namespace GestionHotel.Infrastructure.Data;

public class HotelDbContext : DbContext
{
    public HotelDbContext(DbContextOptions<HotelDbContext> options)
        : base(options) { }

    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<Payment> Payments => Set<Payment>();
}
