using GestionHotel.Domain.Interfaces;
using GestionHotel.Domain.Enums;
using Microsoft.Extensions.Logging;
using GestionHotel.Application.Common;

namespace GestionHotel.Application.UseCases.Booking;

public class MarkRoomAsCleaned
{
    private readonly IReservationRepository _reservationRepository;
    private readonly ILogger<MarkRoomAsCleaned> _logger;

    public MarkRoomAsCleaned(IReservationRepository reservationRepository, ILogger<MarkRoomAsCleaned> logger)
    {
        _reservationRepository = reservationRepository;
        _logger = logger;
    }

    public Result Execute(Guid roomId)
    {
        var reservations = _reservationRepository.GetAll();

        var target = reservations
            .Where(r => r.Status == ReservationStatus.CheckOut)
            .SelectMany(r => r.ReservationRooms
                .Where(rr => rr.RoomId == roomId && !rr.IsCleaned)
                .Select(rr => new { Reservation = r, ResRoom = rr }))
            .OrderByDescending(x => x.Reservation.EndDate)
            .FirstOrDefault();

        if (target == null)
            return Result.Failure("Aucune réservation à nettoyer trouvée pour cette chambre.");

        target.ResRoom.IsCleaned = true;
        _reservationRepository.Update(target.Reservation);

        _logger.LogInformation("Chambre {RoomId} nettoyée pour la réservation {ReservationId}", roomId, target.Reservation.Id);
        return Result.Success("Chambre marquée comme nettoyée.");
    }
}
