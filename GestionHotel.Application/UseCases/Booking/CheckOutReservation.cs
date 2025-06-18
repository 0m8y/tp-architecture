using GestionHotel.Domain.Entities;
using GestionHotel.Domain.Enums;
using GestionHotel.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using GestionHotel.Application.Common;

namespace GestionHotel.Application.UseCases.Booking;

public class CheckOutReservation
{
    private readonly IReservationRepository _reservationRepository;
    private readonly ILogger<CheckOutReservation> _logger;

    public CheckOutReservation(
        IReservationRepository reservationRepository,
        ILogger<CheckOutReservation> logger)
    {
        _reservationRepository = reservationRepository;
        _logger = logger;
    }

    public Result Execute(Guid reservationId)
    {
        var reservation = _reservationRepository.GetById(reservationId);
        if (reservation == null)
            return Result.Failure("Réservation introuvable.");

        if (reservation.Status != ReservationStatus.CheckIn)
            return Result.Failure("Le check-out est possible uniquement après un check-in.");

        foreach (var resRoom in reservation.ReservationRooms)
        {
            resRoom.Room.NeedsCleaning = true;
        }

        // TODO : Gestion des dommages signalés par le personnel de ménage
        // if (damageReported)
        // {
        //     ApplyExtraCharges(reservation, damageDetails);
        //     _logger.LogInformation("Frais supplémentaires appliqués pour la réservation {ReservationId}", reservation.Id);
        // }

        reservation.Status = ReservationStatus.CheckOut;
        _reservationRepository.Update(reservation);

        _logger.LogInformation("Check-out effectué pour la réservation {ReservationId}", reservation.Id);
        return Result.Success("Check-out effectué avec succès. Chambres marquées pour nettoyage.");
    }
}
