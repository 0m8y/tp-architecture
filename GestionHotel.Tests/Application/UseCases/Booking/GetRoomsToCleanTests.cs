using Moq;
using Microsoft.Extensions.Logging;
using GestionHotel.Application.UseCases.Booking;
using GestionHotel.Domain.Entities;
using GestionHotel.Domain.Enums;
using GestionHotel.Domain.Interfaces;

namespace GestionHotel.Tests.UseCases.Booking;

public class GetRoomsToCleanTests
{
    private readonly Mock<IReservationRepository> _reservationRepoMock;
    private readonly Mock<ILogger<GetRoomsToClean>> _loggerMock;
    private readonly GetRoomsToClean _useCase;

    public GetRoomsToCleanTests()
    {
        _reservationRepoMock = new Mock<IReservationRepository>();
        _loggerMock = new Mock<ILogger<GetRoomsToClean>>();
        _useCase = new GetRoomsToClean(_reservationRepoMock.Object, _loggerMock.Object);
    }

    [Fact]
    public void ShouldReturnEmpty_WhenNoReservations()
    {
        _reservationRepoMock.Setup(r => r.GetAll()).Returns(new List<Reservation>());

        var result = _useCase.Execute();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void ShouldReturnEmpty_WhenAllRoomsAlreadyCleaned()
    {
        var reservations = new List<Reservation>
        {
            new Reservation
            {
                Status = ReservationStatus.CheckOut,
                EndDate = DateTime.UtcNow.AddDays(-1),
                ReservationRooms = new List<ReservationRoom>
                {
                    new ReservationRoom
                    {
                        RoomId = Guid.NewGuid(),
                        Room = new Room { Number = "101" },
                        IsCleaned = true
                    }
                }
            }
        };

        _reservationRepoMock.Setup(r => r.GetAll()).Returns(reservations);

        var result = _useCase.Execute();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void ShouldReturnRoomToClean_WhenCheckOutAndNotCleaned()
    {
        var roomId = Guid.NewGuid();
        var endDate = DateTime.UtcNow.AddDays(-1);

        var reservations = new List<Reservation>
        {
            new Reservation
            {
                Status = ReservationStatus.CheckOut,
                EndDate = endDate,
                ReservationRooms = new List<ReservationRoom>
                {
                    new ReservationRoom
                    {
                        RoomId = roomId,
                        Room = new Room { Id = roomId, Number = "101" },
                        IsCleaned = false
                    }
                }
            },
            new Reservation
            {
                Status = ReservationStatus.Confirmed,
                StartDate = DateTime.UtcNow.AddDays(1),
                ReservationRooms = new List<ReservationRoom>
                {
                    new ReservationRoom { RoomId = roomId }
                }
            }
        };

        _reservationRepoMock.Setup(r => r.GetAll()).Returns(reservations);

        var result = _useCase.Execute();

        Assert.Single(result);
        var room = result.First();
        Assert.Equal(roomId, room.RoomId);
        Assert.Equal("101", room.RoomNumber);
        Assert.Equal(endDate, room.LastOccupied);
        Assert.Equal(reservations[1].StartDate, room.NextOccupied);
    }
}
