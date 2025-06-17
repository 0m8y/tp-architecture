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
        var hoursBeforeStart = (reservation.StartDate - now).TotalHours;
        var refundEligible = hoursBeforeStart >= 48;

        reservation.Status = ReservationStatus.Cancelled;
        _reservationRepository.Update(reservation);

        if (reservation.IsPaid && refundEligible)
        {
            var payment = _paymentRepository.GetByReservationId(reservationId);
            if (payment != null)
            {
                payment.IsRefunded = true;
                _paymentRepository.Save(payment);
            }
        }

        _logger.LogInformation("Réservation {ReservationId} annulée. Remboursement : {Refunded}", reservationId, refundEligible);

        return Result.Success();
    }
}
