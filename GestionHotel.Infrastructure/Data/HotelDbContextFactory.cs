using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using GestionHotel.Infrastructure.Data;

namespace GestionHotel.Infrastructure.Data;

public class HotelDbContextFactory : IDesignTimeDbContextFactory<HotelDbContext>
{
    public HotelDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<HotelDbContext>();

        var connectionString = Environment.GetEnvironmentVariable("HOTEL_DB_CONNECTION")
    ?? "server=localhost;port=3306;database=gestion_hotel;user=root;password=azerty";


        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

        return new HotelDbContext(optionsBuilder.Options);
    }
}
