using GestionHotel.Domain.Enums;
using GestionHotel.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace GestionHotel.Application.UseCases.Booking;

public class CancelReservation
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly ILogger<CancelReservation> _logger;

    public CancelReservation(
        IReservationRepository reservationRepository,
        IPaymentRepository paymentRepository,
        ILogger<CancelReservation> logger)
    {
        _reservationRepository = reservationRepository;
        _paymentRepository = paymentRepository;
        _logger = logger;
    }

    public Result Execute(Guid reservationId, Guid clientId)
    {
        var reservation = _reservationRepository.GetById(reservationId);
        if (reservation == null)
            return Result.Failure("Réservation introuvable.");

        if (reservation.ClientId != clientId)
            return Result.Failure("Accès refusé à cette réservation.");

        if (reservation.Status == ReservationStatus.Cancelled)
            return Result.Failure("Réservation déjà annulée.");

        var now = DateTime.UtcNow;

        // On suppose que StartDate est en heure locale => on la convertit correctement en UTC
        var startUtc = TimeZoneInfo.ConvertTimeToUtc(reservation.StartDate, TimeZoneInfo.Local);

        _logger.LogInformation("Annulation demandée à {Now} ({NowKind}), StartDate: {Start} ({StartKind}), Interprété comme UTC: {StartUtc}",
            now, now.Kind, reservation.StartDate, reservation.StartDate.Kind, startUtc);

        var hoursBeforeStart = (startUtc - now).TotalHours;
        var refundEligible = hoursBeforeStart >= 48;

        reservation.Status = ReservationStatus.Cancelled;
        _reservationRepository.Update(reservation);

        if (reservation.IsPaid && refundEligible)
        {
            var payment = _paymentRepository.GetByReservationId(reservationId);
            if (payment != null)
            {
                payment.IsRefunded = true;
                _paymentRepository.Update(payment);

                var refundAmount = reservation.TotalAmount;
                _logger.LogInformation("Réservation {ReservationId} annulée. Remboursement : {Amount}€", reservationId, refundAmount);
                return Result.Success($"Réservation annulée. Un remboursement de {refundAmount:0.00}€ sera effectué.");
            }
            else
            {
                _logger.LogWarning("Réservation {ReservationId} annulée, mais aucun enregistrement de paiement n'a été trouvé pour effectuer le remboursement.", reservationId);
                return Result.Success("Réservation annulée. Aucun remboursement n’a pu être effectué car le paiement n’a pas été retrouvé.");
            }
        }

        _logger.LogInformation("Réservation {ReservationId} annulée. Remboursement non éligible (différence {Hours}h)", reservationId, hoursBeforeStart);

        return Result.Success("Réservation annulée. Aucun remboursement n’est possible car l’annulation a eu lieu moins de 48h avant le début.");
    }
}
