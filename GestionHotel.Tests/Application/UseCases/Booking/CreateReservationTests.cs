using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using GestionHotel.Domain.Entities;
using GestionHotel.Domain.Enums;
using GestionHotel.Domain.Interfaces;
using GestionHotel.Application.UseCases.Booking;
using GestionHotel.Application.Common;
using GestionHotel.Domain.Rules;

namespace GestionHotel.Tests.UseCases.Booking;

public class CreateReservationTests
{
    private readonly Mock<IReservationRepository> _reservationRepoMock;
    private readonly Mock<IRoomRepository> _roomRepoMock;
    private readonly CreateReservation _useCase;

    public CreateReservationTests()
    {
        _reservationRepoMock = new Mock<IReservationRepository>();
        _roomRepoMock = new Mock<IRoomRepository>();
        _useCase = new CreateReservation(_reservationRepoMock.Object, _roomRepoMock.Object);
    }

    [Fact]
    public void Execute_ShouldFail_WhenRoomNotFound()
    {
        var roomId = Guid.NewGuid();

        _roomRepoMock.Setup(r => r.GetWithReservationsById(roomId)).Returns((Room?)null);

        var result = _useCase.Execute(Guid.NewGuid(), DateTime.Today, DateTime.Today.AddDays(1), new List<Guid> { roomId });

        Assert.False(result.IsSuccess);
        Assert.Equal("Une ou plusieurs chambres sont introuvables.", result.ErrorMessage);
    }

    [Fact]
    public void Execute_ShouldFail_WhenRoomOverlaps()
    {
        var roomId = Guid.NewGuid();

        var existingReservation = new Reservation
        {
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(2),
            Status = ReservationStatus.Confirmed
        };

        var room = new Room
        {
            Id = roomId,
            Number = "101",
            Type = RoomType.Single,
            ReservationRooms = new List<ReservationRoom>
            {
                new ReservationRoom
                {
                    RoomId = roomId,
                    Reservation = existingReservation
                }
            }
        };

        _roomRepoMock.Setup(r => r.GetWithReservationsById(roomId)).Returns(room);

        var result = _useCase.Execute(Guid.NewGuid(), DateTime.Today, DateTime.Today.AddDays(1), new List<Guid> { roomId });

        Assert.False(result.IsSuccess);
        Assert.Equal("La chambre 101 est déjà réservée.", result.ErrorMessage);
    }

    [Fact]
    public void Execute_ShouldSucceed_WhenAllIsValid()
    {
        var roomId = Guid.NewGuid();

        var room = new Room
        {
            Id = roomId,
            Number = "101",
            Type = RoomType.Single,
            ReservationRooms = new List<ReservationRoom>()
        };

        _roomRepoMock.Setup(r => r.GetWithReservationsById(roomId)).Returns(room);

        var result = _useCase.Execute(Guid.NewGuid(), DateTime.Today, DateTime.Today.AddDays(1), new List<Guid> { roomId });

        Assert.True(result.IsSuccess);
        Assert.Equal("Réservation créée avec succès.", result.Message);
        _reservationRepoMock.Verify(r => r.Create(It.IsAny<Reservation>()), Times.Once);
    }
}
