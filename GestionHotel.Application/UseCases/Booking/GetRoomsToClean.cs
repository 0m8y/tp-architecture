using GestionHotel.Domain.Enums;
using GestionHotel.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace GestionHotel.Application.UseCases.Booking;

public class RoomToClean
{
    public Guid RoomId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public DateTime LastOccupied { get; set; }
    public DateTime? NextOccupied { get; set; }
}

public class GetRoomsToClean
{
    private readonly IReservationRepository _reservationRepository;
    private readonly ILogger<GetRoomsToClean> _logger;

    public GetRoomsToClean(IReservationRepository reservationRepository, ILogger<GetRoomsToClean> logger)
    {
        _reservationRepository = reservationRepository;
        _logger = logger;
    }

    public List<RoomToClean> Execute()
    {
        var reservations = _reservationRepository.GetAll();

        // Réservations terminées (check-out) dont les chambres ne sont pas encore nettoyées
        var toClean = reservations
            .Where(r => r.Status == ReservationStatus.CheckOut)
            .SelectMany(r => r.ReservationRooms.Select(rr => new
            {
                Room = rr.Room,
                RoomId = rr.RoomId,
                IsCleaned = rr.IsCleaned,
                Reservation = r
            }))
            .Where(x => !x.IsCleaned)
            .Select(x =>
            {
                // Trouver prochaine réservation pour cette chambre
                var next = reservations
                    .Where(r => r.Status == ReservationStatus.Confirmed && r.StartDate > x.Reservation.EndDate)
                    .Where(r => r.ReservationRooms.Any(rr => rr.RoomId == x.RoomId))
                    .OrderBy(r => r.StartDate)
                    .FirstOrDefault();

                return new RoomToClean
                {
                    RoomId = x.RoomId,
                    RoomNumber = x.Room.Number,
                    LastOccupied = x.Reservation.EndDate,
                    NextOccupied = next?.StartDate
                };
            })
            .OrderBy(x => x.NextOccupied ?? DateTime.MaxValue) // priorité à réoccupations proches
            .ThenByDescending(x => x.LastOccupied) // sinon plus récentes en haut
            .ToList();

        _logger.LogInformation("{Count} chambres à nettoyer trouvées", toClean.Count);
        return toClean;
    }
}
