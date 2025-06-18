using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using GestionHotel.Application.UseCases.Booking;
using GestionHotel.Domain.Entities;
using GestionHotel.Domain.Enums;
using GestionHotel.Domain.Interfaces;
using GestionHotel.Application.Common;

namespace GestionHotel.Tests.UseCases.Booking;

public class MarkRoomAsCleanedTests
{
    private readonly Mock<IReservationRepository> _reservationRepoMock;
    private readonly Mock<ILogger<MarkRoomAsCleaned>> _loggerMock;
    private readonly MarkRoomAsCleaned _useCase;

    public MarkRoomAsCleanedTests()
    {
        _reservationRepoMock = new Mock<IReservationRepository>();
        _loggerMock = new Mock<ILogger<MarkRoomAsCleaned>>();
        _useCase = new MarkRoomAsCleaned(_reservationRepoMock.Object, _loggerMock.Object);
    }

    [Fact]
    public void ShouldReturnFailure_WhenNoReservationFound()
    {
        _reservationRepoMock.Setup(r => r.GetAll()).Returns(new List<Reservation>());

        var result = _useCase.Execute(Guid.NewGuid());

        Assert.False(result.IsSuccess);
        Assert.Equal("Aucune réservation à nettoyer trouvée pour cette chambre.", result.ErrorMessage);
    }

    [Fact]
    public void ShouldReturnFailure_WhenRoomAlreadyCleaned()
    {
        var roomId = Guid.NewGuid();

        var reservations = new List<Reservation>
        {
            new Reservation
            {
                Id = Guid.NewGuid(),
                Status = ReservationStatus.CheckOut,
                ReservationRooms = new List<ReservationRoom>
                {
                    new ReservationRoom
                    {
                        RoomId = roomId,
                        IsCleaned = true
                    }
                }
            }
        };

        _reservationRepoMock.Setup(r => r.GetAll()).Returns(reservations);

        var result = _useCase.Execute(roomId);

        Assert.False(result.IsSuccess);
        Assert.Equal("Aucune réservation à nettoyer trouvée pour cette chambre.", result.ErrorMessage);
    }

    [Fact]
    public void ShouldMarkRoomAsCleaned_WhenReservationExistsAndNotCleaned()
    {
        var roomId = Guid.NewGuid();
        var reservation = new Reservation
        {
            Id = Guid.NewGuid(),
            Status = ReservationStatus.CheckOut,
            ReservationRooms = new List<ReservationRoom>
            {
                new ReservationRoom
                {
                    RoomId = roomId,
                    IsCleaned = false
                }
            }
        };

        _reservationRepoMock.Setup(r => r.GetAll()).Returns(new List<Reservation> { reservation });

        var result = _useCase.Execute(roomId);

        Assert.True(result.IsSuccess);
        Assert.Equal("Chambre marquée comme nettoyée.", result.Message);
        Assert.True(reservation.ReservationRooms.First().IsCleaned);
        _reservationRepoMock.Verify(r => r.Update(reservation), Times.Once);
    }
}
