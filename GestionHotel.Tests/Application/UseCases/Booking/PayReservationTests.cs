using Xunit;
using Moq;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using GestionHotel.Application.UseCases.Booking;
using GestionHotel.Domain.Interfaces;
using GestionHotel.Domain.Entities;
using GestionHotel.Application.Factory;
using GestionHotel.Domain.Enums;
using GestionHotel.Application.Common;

namespace GestionHotel.Tests.UseCases.Booking;

public class PayReservationTests
{
    private readonly Mock<IReservationRepository> _reservationRepoMock;
    private readonly Mock<IPaymentRepository> _paymentRepoMock;
    private readonly Mock<IPaymentGatewayFactory> _gatewayFactoryMock;
    private readonly Mock<ILogger<PayReservation>> _loggerMock;
    private readonly Mock<IPaymentGateway> _gatewayMock;
    private readonly PayReservation _useCase;

    public PayReservationTests()
    {
        _reservationRepoMock = new Mock<IReservationRepository>();
        _paymentRepoMock = new Mock<IPaymentRepository>();
        _gatewayFactoryMock = new Mock<IPaymentGatewayFactory>();
        _loggerMock = new Mock<ILogger<PayReservation>>();
        _gatewayMock = new Mock<IPaymentGateway>();

        _gatewayFactoryMock.Setup(f => f.Get(It.IsAny<PaymentProvider>())).Returns(_gatewayMock.Object);

        _useCase = new PayReservation(
            _reservationRepoMock.Object,
            _paymentRepoMock.Object,
            _gatewayFactoryMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnFailure_WhenReservationNotFound()
    {
        _reservationRepoMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns((Reservation?)null);

        var result = await _useCase.ExecuteAsync(Guid.NewGuid(), "4111111111111111", "12/26", PaymentProvider.Stripe);

        Assert.False(result.IsSuccess);
        Assert.Equal("Réservation introuvable.", result.ErrorMessage);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnFailure_WhenReservationAlreadyPaid()
    {
        var reservation = new Reservation { IsPaid = true };
        _reservationRepoMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns(reservation);

        var result = await _useCase.ExecuteAsync(Guid.NewGuid(), "4111111111111111", "12/26", PaymentProvider.Stripe);

        Assert.False(result.IsSuccess);
        Assert.Equal("Cette réservation a déjà été payée.", result.ErrorMessage);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnFailure_WhenCardExpiryInvalid()
    {
        var reservation = new Reservation { IsPaid = false };
        _reservationRepoMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns(reservation);

        var result = await _useCase.ExecuteAsync(Guid.NewGuid(), "4111111111111111", "01/19", PaymentProvider.Stripe);

        Assert.False(result.IsSuccess);
        Assert.Equal("Date d'expiration invalide.", result.ErrorMessage);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnFailure_WhenPaymentFails()
    {
        var reservation = new Reservation { IsPaid = false, TotalAmount = 100 };
        _reservationRepoMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns(reservation);
        _gatewayMock.Setup(g => g.ProcessPaymentAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>())).ReturnsAsync(false);

        var result = await _useCase.ExecuteAsync(Guid.NewGuid(), "4111111111111111", "12/30", PaymentProvider.Stripe);

        Assert.False(result.IsSuccess);
        Assert.Equal("Échec du paiement.", result.ErrorMessage);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnSuccess_WhenPaymentSucceeds()
    {
        var reservation = new Reservation
        {
            Id = Guid.NewGuid(),
            IsPaid = false,
            TotalAmount = 200,
            Status = ReservationStatus.PendingPayment,
            ReservationRooms = new List<ReservationRoom>()
        };

        _reservationRepoMock.Setup(r => r.GetById(reservation.Id)).Returns(reservation);
        _gatewayMock.Setup(g => g.ProcessPaymentAsync(It.IsAny<string>(), It.IsAny<string>(), reservation.TotalAmount)).ReturnsAsync(true);

        var result = await _useCase.ExecuteAsync(reservation.Id, "4111111111111111", "12/30", PaymentProvider.Paypal);

        Assert.True(result.IsSuccess);
        Assert.Equal("Paiement effectué avec succès.", result.Message);
        _reservationRepoMock.Verify(r => r.Update(It.Is<Reservation>(res => res.IsPaid && res.Status == ReservationStatus.Confirmed)), Times.Once);
        _paymentRepoMock.Verify(p => p.Add(It.IsAny<Payment>()), Times.Once);
    }
}
