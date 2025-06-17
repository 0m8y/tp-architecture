using GestionHotel.Domain.Interfaces;
using GestionHotel.Domain.Enums;
using Microsoft.Extensions.Logging;
using GestionHotel.Application.Factory;
using GestionHotel.Application.Validators;

namespace GestionHotel.Application.UseCases.Booking;

public class PayReservation
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IPaymentGatewayFactory _paymentGatewayFactory;
    private readonly ILogger<PayReservation> _logger;

    public PayReservation(
        IReservationRepository reservationRepository,
        IPaymentGatewayFactory paymentGatewayFactory,
        ILogger<PayReservation> logger)
    {
        _reservationRepository = reservationRepository;
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
        _logger.LogInformation("Paiement réussi pour la réservation {ReservationId}", reservationId);

        return Result.Success();
    }
}

public class Result
{
    public bool IsSuccess { get; private set; }
    public string? ErrorMessage { get; private set; }

    public static Result Success() => new Result { IsSuccess = true };
    public static Result Failure(string errorMessage) => new Result { IsSuccess = false, ErrorMessage = errorMessage };
}
