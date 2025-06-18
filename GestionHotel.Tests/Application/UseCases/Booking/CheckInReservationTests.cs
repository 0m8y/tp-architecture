using Xunit;
using Moq;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using GestionHotel.Application.UseCases.Booking;
using GestionHotel.Domain.Entities;
using GestionHotel.Domain.Enums;
using GestionHotel.Domain.Interfaces;
using GestionHotel.Application.Common;
using GestionHotel.Application.Factory;

namespace GestionHotel.Tests.UseCases.Booking;

public class CheckInReservationTests
{
    private readonly Mock<IReservationRepository> _reservationRepoMock;
    private readonly Mock<IPaymentRepository> _paymentRepoMock;
    private readonly Mock<IPaymentGatewayFactory> _gatewayFactoryMock;
    private readonly Mock<IPaymentGateway> _paymentGatewayMock;
    private readonly Mock<ILogger<CheckInReservation>> _loggerMock;
    private readonly CheckInReservation _useCase;

    public CheckInReservationTests()
    {
        _reservationRepoMock = new Mock<IReservationRepository>();
        _paymentRepoMock = new Mock<IPaymentRepository>();
        _gatewayFactoryMock = new Mock<IPaymentGatewayFactory>();
        _paymentGatewayMock = new Mock<IPaymentGateway>();
        _loggerMock = new Mock<ILogger<CheckInReservation>>();

        _gatewayFactoryMock
            .Setup(f => f.Get(It.IsAny<PaymentProvider>()))
            .Returns(_paymentGatewayMock.Object);

        _useCase = new CheckInReservation(
            _reservationRepoMock.Object,
            _paymentRepoMock.Object,
            _gatewayFactoryMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task ShouldFail_WhenReservationNotFound()
    {
        _reservationRepoMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns((Reservation?)null);

        var result = await _useCase.ExecuteAsync(Guid.NewGuid(), null, null, null);

        Assert.False(result.IsSuccess);
        Assert.Equal("Réservation introuvable.", result.ErrorMessage);
    }

    [Theory]
    [InlineData(ReservationStatus.Cancelled, "La réservation est annulée.")]
    [InlineData(ReservationStatus.CheckOut, "La réservation est terminée.")]
    [InlineData(ReservationStatus.CheckIn, "Le client a déjà effectué le check-in.")]
    public async Task ShouldFail_WhenInvalidStatus(ReservationStatus status, string expected)
    {
        var reservation = new Reservation { Status = status };
        _reservationRepoMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns(reservation);

        var result = await _useCase.ExecuteAsync(Guid.NewGuid(), null, null, null);

        Assert.False(result.IsSuccess);
        Assert.Equal(expected, result.ErrorMessage);
    }

    [Fact]
    public async Task ShouldFail_WhenPaymentRequired_ButMissingInfos()
    {
        var reservation = new Reservation
        {
            Status = ReservationStatus.Confirmed,
            IsPaid = false
        };

        _reservationRepoMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns(reservation);

        var result = await _useCase.ExecuteAsync(Guid.NewGuid(), null, null, null);

        Assert.False(result.IsSuccess);
        Assert.Contains("Informations de paiement manquantes", result.ErrorMessage);
    }

    [Fact]
    public async Task ShouldFail_WhenPaymentFails()
    {
        var reservation = new Reservation
        {
            Id = Guid.NewGuid(),
            Status = ReservationStatus.Confirmed,
            IsPaid = false,
            TotalAmount = 100
        };

        _reservationRepoMock.Setup(r => r.GetById(reservation.Id)).Returns(reservation);
        _paymentGatewayMock
            .Setup(p => p.ProcessPaymentAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>()))
            .ReturnsAsync(false);

        var result = await _useCase.ExecuteAsync(reservation.Id, "4111111111111111", "12/30", PaymentProvider.Stripe);

        Assert.False(result.IsSuccess);
        Assert.Equal("Échec du paiement.", result.ErrorMessage);
    }

    [Fact]
    public async Task ShouldSucceed_WhenPaymentSucceeds()
    {
        var reservation = new Reservation
        {
            Id = Guid.NewGuid(),
            Status = ReservationStatus.Confirmed,
            IsPaid = false,
            TotalAmount = 150
        };

        _reservationRepoMock.Setup(r => r.GetById(reservation.Id)).Returns(reservation);
        _paymentGatewayMock
            .Setup(p => p.ProcessPaymentAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>()))
            .ReturnsAsync(true);

        var result = await _useCase.ExecuteAsync(reservation.Id, "4111111111111111", "12/30", PaymentProvider.Paypal);

        Assert.True(result.IsSuccess);
        Assert.Equal("Check-in effectué avec succès.", result.Message);
        Assert.True(reservation.IsPaid);
        Assert.Equal(ReservationStatus.CheckIn, reservation.Status);
        _reservationRepoMock.Verify(r => r.Update(reservation), Times.Once);
        _paymentRepoMock.Verify(p => p.Add(It.IsAny<Payment>()), Times.Once);
    }

    [Fact]
    public async Task ShouldSucceed_WhenAlreadyPaid()
    {
        var reservation = new Reservation
        {
            Id = Guid.NewGuid(),
            Status = ReservationStatus.Confirmed,
            IsPaid = true
        };

        _reservationRepoMock.Setup(r => r.GetById(reservation.Id)).Returns(reservation);

        var result = await _useCase.ExecuteAsync(reservation.Id, null, null, null);

        Assert.True(result.IsSuccess);
        Assert.Equal("Check-in effectué avec succès.", result.Message);
        Assert.Equal(ReservationStatus.CheckIn, reservation.Status);
        _reservationRepoMock.Verify(r => r.Update(reservation), Times.Once);
        _paymentRepoMock.Verify(p => p.Add(It.IsAny<Payment>()), Times.Never);
    }
}
