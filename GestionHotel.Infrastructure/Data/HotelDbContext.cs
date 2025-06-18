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
    public DbSet<ReservationRoom> ReservationRooms => Set<ReservationRoom>();
    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client>()
            .HasIndex(c => c.Email)
            .IsUnique();

        modelBuilder.Entity<ReservationRoom>()
            .HasKey(rr => new { rr.ReservationId, rr.RoomId });

        modelBuilder.Entity<ReservationRoom>()
            .HasOne(rr => rr.Reservation)
            .WithMany(r => r.ReservationRooms)
            .HasForeignKey(rr => rr.ReservationId);

        modelBuilder.Entity<ReservationRoom>()
            .HasOne(rr => rr.Room)
            .WithMany(r => r.ReservationRooms)
            .HasForeignKey(rr => rr.RoomId);

        modelBuilder.Entity<Client>().HasData(
            new Client
            {
                Id = Guid.Parse("22222222-1111-1111-1111-111111111111"),
                Email = "client@mail.com",
                Password = "$2a$11$Z2hhARDoUFkWGfmAQ3dDWehkNupOwBpOFl8bZ444iHOujoSn248Sy",
                Role = Role.Client
            },
            new Client
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Email = "receptionist@mail.com",
                Password = "$2a$11$Z2hhARDoUFkWGfmAQ3dDWehkNupOwBpOFl8bZ444iHOujoSn248Sy",
                Role = Role.Receptionist
            },
            new Client
            {
                Id = Guid.Parse("22222222-3333-3333-3333-333333333333"),
                Email = "cleaner@mail.com",
                Password = "$2a$11$Z2hhARDoUFkWGfmAQ3dDWehkNupOwBpOFl8bZ444iHOujoSn248Sy",
                Role = Role.Cleaner
            }
        );

        modelBuilder.Entity<Room>().HasData(
            new Room { Id = Guid.Parse("11111111-1111-1111-1111-111111111101"), Number = "101", Capacity = 1, Type = RoomType.Single, Condition = RoomCondition.New },
            new Room { Id = Guid.Parse("11111111-1111-1111-1111-111111111102"), Number = "102", Capacity = 1, Type = RoomType.Single, Condition = RoomCondition.New },
            new Room { Id = Guid.Parse("11111111-1111-1111-1111-111111111103"), Number = "103", Capacity = 2, Type = RoomType.Double, Condition = RoomCondition.NoIssues },
            new Room { Id = Guid.Parse("11111111-1111-1111-1111-111111111104"), Number = "104", Capacity = 2, Type = RoomType.Double, Condition = RoomCondition.NoIssues },
            new Room { Id = Guid.Parse("11111111-1111-1111-1111-111111111105"), Number = "105", Capacity = 2, Type = RoomType.Double, Condition = RoomCondition.Refurbished },
            new Room { Id = Guid.Parse("11111111-1111-1111-1111-111111111106"), Number = "106", Capacity = 3, Type = RoomType.Suite, Condition = RoomCondition.New },
            new Room { Id = Guid.Parse("11111111-1111-1111-1111-111111111107"), Number = "107", Capacity = 3, Type = RoomType.Suite, Condition = RoomCondition.NoIssues },
            new Room { Id = Guid.Parse("11111111-1111-1111-1111-111111111108"), Number = "108", Capacity = 4, Type = RoomType.Suite, Condition = RoomCondition.Refurbished },
            new Room { Id = Guid.Parse("11111111-1111-1111-1111-111111111109"), Number = "109", Capacity = 4, Type = RoomType.Suite, Condition = RoomCondition.NoIssues },
            new Room { Id = Guid.Parse("11111111-1111-1111-1111-111111111110"), Number = "110", Capacity = 4, Type = RoomType.Suite, Condition = RoomCondition.New }
        );
    }
}
