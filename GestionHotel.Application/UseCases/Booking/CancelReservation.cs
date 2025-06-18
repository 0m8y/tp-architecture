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

    public Result Execute(Guid reservationId, Guid clientId, bool isReceptionist = false, bool forceRefund = false)
    {
        var reservation = _reservationRepository.GetById(reservationId);
        if (reservation == null)
            return Result.Failure("Réservation introuvable.");

        if (!isReceptionist && reservation.ClientId != clientId)
            return Result.Failure("Accès refusé à cette réservation.");

        if (reservation.Status == ReservationStatus.Cancelled)
            return Result.Failure("Réservation déjà annulée.");

        if (reservation.Status == ReservationStatus.CheckIn)
            return Result.Failure("Impossible d'annuler une réservation déjà enregistrée (check-in effectué).");

        var now = DateTime.UtcNow;
        var startUtc = TimeZoneInfo.ConvertTimeToUtc(reservation.StartDate, TimeZoneInfo.Local);
        var hoursBeforeStart = (startUtc - now).TotalHours;
        var refundEligible = hoursBeforeStart >= 48 || (isReceptionist && forceRefund);

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
                _logger.LogWarning("Réservation {ReservationId} annulée, mais aucun paiement retrouvé.", reservationId);
                return Result.Success("Réservation annulée. Aucun remboursement possible (paiement non retrouvé).");
            }
        }

        _logger.LogInformation("Réservation {ReservationId} annulée. Aucun remboursement (différence {Hours}h)", reservationId, hoursBeforeStart);
        return Result.Success("Réservation annulée. Aucun remboursement possible.");
    }


}
