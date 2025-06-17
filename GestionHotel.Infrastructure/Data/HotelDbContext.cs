using Microsoft.EntityFrameworkCore;
using GestionHotel.Domain.Entities;
using GestionHotel.Domain.Enums;

namespace GestionHotel.Infrastructure.Data;

public class HotelDbContext : DbContext
{
    public HotelDbContext(DbContextOptions<HotelDbContext> options)
        : base(options) { }

    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<Payment> Payments => Set<Payment>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client>()
            .HasIndex(c => c.Email)
            .IsUnique();

        modelBuilder.Entity<Reservation>()
            .HasOne<Room>()
            .WithMany()
            .HasForeignKey(r => r.RoomId);

        modelBuilder.Entity<Room>().HasData(
            new Room { Id = Guid.Parse("11111111-1111-1111-1111-111111111101"), Number = "101", Capacity = 1, Price = 60m, Type = RoomType.Single, Condition = RoomCondition.New },
            new Room { Id = Guid.Parse("11111111-1111-1111-1111-111111111102"), Number = "102", Capacity = 1, Price = 65m, Type = RoomType.Single, Condition = RoomCondition.New },
            new Room { Id = Guid.Parse("11111111-1111-1111-1111-111111111103"), Number = "103", Capacity = 2, Price = 80m, Type = RoomType.Double, Condition = RoomCondition.NoIssues },
            new Room { Id = Guid.Parse("11111111-1111-1111-1111-111111111104"), Number = "104", Capacity = 2, Price = 85m, Type = RoomType.Double, Condition = RoomCondition.NoIssues },
            new Room { Id = Guid.Parse("11111111-1111-1111-1111-111111111105"), Number = "105", Capacity = 2, Price = 90m, Type = RoomType.Double, Condition = RoomCondition.Refurbished },
            new Room { Id = Guid.Parse("11111111-1111-1111-1111-111111111106"), Number = "106", Capacity = 3, Price = 110m, Type = RoomType.Suite, Condition = RoomCondition.New },
            new Room { Id = Guid.Parse("11111111-1111-1111-1111-111111111107"), Number = "107", Capacity = 3, Price = 115m, Type = RoomType.Suite, Condition = RoomCondition.NoIssues },
            new Room { Id = Guid.Parse("11111111-1111-1111-1111-111111111108"), Number = "108", Capacity = 4, Price = 140m, Type = RoomType.Suite, Condition = RoomCondition.Refurbished },
            new Room { Id = Guid.Parse("11111111-1111-1111-1111-111111111109"), Number = "109", Capacity = 4, Price = 145m, Type = RoomType.Suite, Condition = RoomCondition.NoIssues },
            new Room { Id = Guid.Parse("11111111-1111-1111-1111-111111111110"), Number = "110", Capacity = 4, Price = 150m, Type = RoomType.Suite, Condition = RoomCondition.New }
        );
    }
}
