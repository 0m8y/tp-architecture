using GestionHotel.Application.Factory;
using GestionHotel.Application.Validators;
using GestionHotel.Domain.Entities;
using GestionHotel.Domain.Enums;
using GestionHotel.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using GestionHotel.Application.Common;

namespace GestionHotel.Application.UseCases.Booking;

public class CheckInReservation
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentGatewayFactory _paymentGatewayFactory;
    private readonly ILogger<CheckInReservation> _logger;

    public CheckInReservation(
        IReservationRepository reservationRepository,
        IPaymentRepository paymentRepository,
        IPaymentGatewayFactory paymentGatewayFactory,
        ILogger<CheckInReservation> logger)
    {
        _reservationRepository = reservationRepository;
        _paymentRepository = paymentRepository;
        _paymentGatewayFactory = paymentGatewayFactory;
        _logger = logger;
    }

    public async Task<Result> ExecuteAsync(Guid reservationId, string? cardNumber, string? expiryDate, PaymentProvider? provider)
    {
        var reservation = _reservationRepository.GetById(reservationId);
        if (reservation == null)
            return Result.Failure("Réservation introuvable.");

        if (reservation.Status == ReservationStatus.Cancelled)
            return Result.Failure("La réservation est annulée.");

        if (reservation.Status == ReservationStatus.CheckOut)
            return Result.Failure("La réservation est terminée.");

        if (reservation.Status == ReservationStatus.CheckIn)
            return Result.Failure("Le client a déjà effectué le check-in.");

        if (!string.IsNullOrWhiteSpace(expiryDate) && !CardValidator.IsExpiryDateValid(expiryDate))
        {
            _logger.LogWarning("Date d'expiration invalide pour la réservation {ReservationId}", reservationId);
            return Result.Failure("Date d'expiration invalide.");
        }

        if (!reservation.IsPaid)
        {
            if (string.IsNullOrWhiteSpace(cardNumber) || string.IsNullOrWhiteSpace(expiryDate) || provider == null)
            {
                return Result.Failure("Informations de paiement manquantes pour effectuer le check-in.");
            }

            var gateway = _paymentGatewayFactory.Get(provider.Value);
            var success = await gateway.ProcessPaymentAsync(cardNumber, expiryDate, reservation.TotalAmount);
            if (!success)
                return Result.Failure("Échec du paiement.");

            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                ReservationId = reservation.Id,
                Amount = reservation.TotalAmount,
                PaymentDate = DateTime.UtcNow,
                IsRefunded = false
            };
            _paymentRepository.Add(payment);

            reservation.IsPaid = true;
        }

        reservation.Status = ReservationStatus.CheckIn;
        _reservationRepository.Update(reservation);

        _logger.LogInformation("Check-in effectué pour la réservation {ReservationId}", reservation.Id);
        return Result.Success("Check-in effectué avec succès.");
    }

}
