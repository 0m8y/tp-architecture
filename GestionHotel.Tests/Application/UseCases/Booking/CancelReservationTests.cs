using Xunit;
using Moq;
using System;
using Microsoft.Extensions.Logging;
using GestionHotel.Application.UseCases.Booking;
using GestionHotel.Domain.Entities;
using GestionHotel.Domain.Enums;
using GestionHotel.Domain.Interfaces;
using GestionHotel.Application.Common;

namespace GestionHotel.Tests.UseCases.Booking;

public class CancelReservationTests
{
    private readonly Mock<IReservationRepository> _reservationRepoMock;
    private readonly Mock<IPaymentRepository> _paymentRepoMock;
    private readonly Mock<ILogger<CancelReservation>> _loggerMock;
    private readonly CancelReservation _useCase;

    public CancelReservationTests()
    {
        _reservationRepoMock = new Mock<IReservationRepository>();
        _paymentRepoMock = new Mock<IPaymentRepository>();
        _loggerMock = new Mock<ILogger<CancelReservation>>();

        _useCase = new CancelReservation(
            _reservationRepoMock.Object,
            _paymentRepoMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public void ShouldFail_WhenReservationNotFound()
    {
        _reservationRepoMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns((Reservation?)null);

        var result = _useCase.Execute(Guid.NewGuid(), Guid.NewGuid());

        Assert.False(result.IsSuccess);
        Assert.Equal("Réservation introuvable.", result.ErrorMessage);
    }

    [Fact]
    public void ShouldFail_WhenClientIsNotOwner()
    {
        var reservation = new Reservation { ClientId = Guid.NewGuid() };
        _reservationRepoMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns(reservation);

        var result = _useCase.Execute(Guid.NewGuid(), Guid.NewGuid());

        Assert.False(result.IsSuccess);
        Assert.Equal("Accès refusé à cette réservation.", result.ErrorMessage);
    }

    [Theory]
    [InlineData(ReservationStatus.Cancelled, "Réservation déjà annulée.")]
    [InlineData(ReservationStatus.CheckOut, "Réservation déjà terminée.")]
    [InlineData(ReservationStatus.CheckIn, "Impossible d'annuler une réservation déjà enregistrée (check-in effectué).")]
    public void ShouldFail_WhenStatusPreventsCancellation(ReservationStatus status, string expectedMessage)
    {
        var clientId = Guid.NewGuid();
        var reservation = new Reservation { ClientId = clientId, Status = status };
        _reservationRepoMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns(reservation);

        var result = _useCase.Execute(Guid.NewGuid(), clientId);

        Assert.False(result.IsSuccess);
        Assert.Equal(expectedMessage, result.ErrorMessage);
    }

    [Fact]
    public void ShouldSucceedWithoutRefund_WhenNotEligible()
    {
        var clientId = Guid.NewGuid();
        var reservation = new Reservation
        {
            Id = Guid.NewGuid(),
            ClientId = clientId,
            Status = ReservationStatus.Confirmed,
            StartDate = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(12), DateTimeKind.Local),
            IsPaid = true,
            TotalAmount = 100
        };

        _reservationRepoMock.Setup(r => r.GetById(reservation.Id)).Returns(reservation);

        var result = _useCase.Execute(reservation.Id, clientId);

        Assert.True(result.IsSuccess);
        Assert.Contains("Aucun remboursement possible", result.Message);
    }

    [Fact]
    public void ShouldRefund_WhenMoreThan48hBeforeStart()
    {
        var clientId = Guid.NewGuid();
        var reservation = new Reservation
        {
            Id = Guid.NewGuid(),
            ClientId = clientId,
            Status = ReservationStatus.Confirmed,
            StartDate = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(72), DateTimeKind.Local),
            IsPaid = true,
            TotalAmount = 150
        };

        var payment = new Payment { ReservationId = reservation.Id };

        _reservationRepoMock.Setup(r => r.GetById(reservation.Id)).Returns(reservation);
        _paymentRepoMock.Setup(p => p.GetByReservationId(reservation.Id)).Returns(payment);

        var result = _useCase.Execute(reservation.Id, clientId);

        Assert.True(result.IsSuccess);
        Assert.Contains("Remboursement", result.Message!, StringComparison.OrdinalIgnoreCase);
        Assert.True(payment.IsRefunded);
        _paymentRepoMock.Verify(p => p.Update(payment), Times.Once);
    }

    [Fact]
    public void ShouldRefund_WhenReceptionistForcesRefund()
    {
        var reservation = new Reservation
        {
            Id = Guid.NewGuid(),
            ClientId = Guid.NewGuid(),
            Status = ReservationStatus.Confirmed,
            StartDate = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(4), DateTimeKind.Local),
            IsPaid = true,
            TotalAmount = 80
        };

        var payment = new Payment { ReservationId = reservation.Id };

        _reservationRepoMock.Setup(r => r.GetById(reservation.Id)).Returns(reservation);
        _paymentRepoMock.Setup(p => p.GetByReservationId(reservation.Id)).Returns(payment);

        var result = _useCase.Execute(reservation.Id, Guid.NewGuid(), isReceptionist: true, forceRefund: true);

        Assert.True(result.IsSuccess);
        Assert.Contains("Remboursement", result.Message!, StringComparison.OrdinalIgnoreCase);
        Assert.True(payment.IsRefunded);
    }

    [Fact]
    public void ShouldHandleMissingPayment_WhenRefundEligible()
    {
        var clientId = Guid.NewGuid();
        var reservation = new Reservation
        {
            Id = Guid.NewGuid(),
            ClientId = clientId,
            Status = ReservationStatus.Confirmed,
            StartDate = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(72), DateTimeKind.Local),
            IsPaid = true,
            TotalAmount = 150
        };

        _reservationRepoMock.Setup(r => r.GetById(reservation.Id)).Returns(reservation);
        _paymentRepoMock.Setup(p => p.GetByReservationId(reservation.Id)).Returns((Payment?)null);

        var result = _useCase.Execute(reservation.Id, clientId);

        Assert.True(result.IsSuccess);
        Assert.Contains("paiement non retrouvé", result.Message);
    }
}
