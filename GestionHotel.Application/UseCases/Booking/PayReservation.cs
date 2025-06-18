using GestionHotel.Application.Factory;
using GestionHotel.Application.Validators;
using GestionHotel.Domain.Entities;
using GestionHotel.Domain.Enums;
using GestionHotel.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace GestionHotel.Application.UseCases.Booking;

public class PayReservation
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentGatewayFactory _paymentGatewayFactory;
    private readonly ILogger<PayReservation> _logger;

    public PayReservation(
        IReservationRepository reservationRepository,
        IPaymentRepository paymentRepository,
        IPaymentGatewayFactory paymentGatewayFactory,
        ILogger<PayReservation> logger)
    {
        _reservationRepository = reservationRepository;
        _paymentRepository = paymentRepository;
        _paymentGatewayFactory = paymentGatewayFactory;
        _logger = logger;
    }

    public async Task<Result> ExecuteAsync(Guid reservationId, string cardNumber, string expiryDate, PaymentProvider provider)
    {
        var reservation = _reservationRepository.GetById(reservationId);

        if (reservation == null)
        {
            _logger.LogWarning("Reservation not found: {ReservationId}", reservationId);
            return Result.Failure("Réservation introuvable.");
        }

        if (reservation.IsPaid)
        {
            _logger.LogInformation("Reservation already paid: {ReservationId}", reservationId);
            return Result.Failure("Cette réservation a déjà été payée.");
        }

        if (!CardValidator.IsExpiryDateValid(expiryDate))
        {
            _logger.LogWarning("Date d'expiration invalide pour la réservation {ReservationId}", reservationId);
            return Result.Failure("Date d'expiration invalide.");
        }

        var gateway = _paymentGatewayFactory.Get(provider);
        var success = await gateway.ProcessPaymentAsync(cardNumber, expiryDate, reservation.TotalAmount);
        if (!success)
        {
            _logger.LogError("Échec du paiement pour la réservation {ReservationId}", reservationId);
            return Result.Failure("Échec du paiement.");
        }

        reservation.IsPaid = true;
        reservation.Status = ReservationStatus.Confirmed;
        _reservationRepository.Update(reservation);

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            ReservationId = reservation.Id,
            PaymentDate = DateTime.UtcNow,
            Amount = reservation.TotalAmount,
            IsRefunded = false
        };
        _paymentRepository.Add(payment);

        _logger.LogInformation("Paiement réussi et enregistré pour la réservation {ReservationId}", reservationId);

        return Result.Success("Paiement effectué avec succès.");
    }
}


public class Result
{
    public bool IsSuccess { get; private set; }
    public string? ErrorMessage { get; private set; }
    public string? Message { get; private set; }

    public static Result Success() => new Result { IsSuccess = true };

    public static Result Success(string message) => new Result { IsSuccess = true, Message = message };

    public static Result Failure(string errorMessage) => new Result { IsSuccess = false, ErrorMessage = errorMessage };
}
