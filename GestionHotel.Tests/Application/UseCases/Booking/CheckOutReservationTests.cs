using Xunit;
using Moq;
using System;
using Microsoft.Extensions.Logging;
using GestionHotel.Application.UseCases.Booking;
using GestionHotel.Domain.Entities;
using GestionHotel.Domain.Enums;
using GestionHotel.Domain.Interfaces;
using GestionHotel.Application.Common;
using System.Collections.Generic;

namespace GestionHotel.Tests.UseCases.Booking;

public class CheckOutReservationTests
{
    private readonly Mock<IReservationRepository> _reservationRepoMock;
    private readonly Mock<ILogger<CheckOutReservation>> _loggerMock;
    private readonly CheckOutReservation _useCase;

    public CheckOutReservationTests()
    {
        _reservationRepoMock = new Mock<IReservationRepository>();
        _loggerMock = new Mock<ILogger<CheckOutReservation>>();
        _useCase = new CheckOutReservation(_reservationRepoMock.Object, _loggerMock.Object);
    }

    [Fact]
    public void ShouldFail_WhenReservationNotFound()
    {
        _reservationRepoMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns((Reservation?)null);

        var result = _useCase.Execute(Guid.NewGuid());

        Assert.False(result.IsSuccess);
        Assert.Equal("Réservation introuvable.", result.ErrorMessage);
    }

    [Theory]
    [InlineData(ReservationStatus.PendingPayment)]
    [InlineData(ReservationStatus.Confirmed)]
    [InlineData(ReservationStatus.Cancelled)]
    [InlineData(ReservationStatus.CheckOut)]
    public void ShouldFail_WhenNotInCheckInStatus(ReservationStatus status)
    {
        var reservation = new Reservation
        {
            Id = Guid.NewGuid(),
            Status = status
        };

        _reservationRepoMock.Setup(r => r.GetById(reservation.Id)).Returns(reservation);

        var result = _useCase.Execute(reservation.Id);

        Assert.False(result.IsSuccess);
        Assert.Equal("Le check-out est possible uniquement après un check-in.", result.ErrorMessage);
    }

    [Fact]
    public void ShouldSucceed_WhenCheckOutIsValid()
    {
        var room1 = new Room { Id = Guid.NewGuid(), NeedsCleaning = false };
        var room2 = new Room { Id = Guid.NewGuid(), NeedsCleaning = false };

        var reservation = new Reservation
        {
            Id = Guid.NewGuid(),
            Status = ReservationStatus.CheckIn,
            ReservationRooms = new List<ReservationRoom>
            {
                new ReservationRoom { Room = room1 },
                new ReservationRoom { Room = room2 }
            }
        };

        _reservationRepoMock.Setup(r => r.GetById(reservation.Id)).Returns(reservation);

        var result = _useCase.Execute(reservation.Id);

        Assert.True(result.IsSuccess);
        Assert.Equal("Check-out effectué avec succès. Chambres marquées pour nettoyage.", result.Message);
        Assert.Equal(ReservationStatus.CheckOut, reservation.Status);
        Assert.True(room1.NeedsCleaning);
        Assert.True(room2.NeedsCleaning);
        _reservationRepoMock.Verify(r => r.Update(reservation), Times.Once);
    }
}
