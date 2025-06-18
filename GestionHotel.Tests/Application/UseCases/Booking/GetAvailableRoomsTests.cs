using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using GestionHotel.Domain.Entities;
using GestionHotel.Domain.Interfaces;
using GestionHotel.Application.UseCases.Booking;

namespace GestionHotel.Tests.UseCases.Booking;

public class GetAvailableRoomsTests
{
    private readonly Mock<IRoomRepository> _roomRepoMock;
    private readonly GetAvailableRooms _useCase;

    public GetAvailableRoomsTests()
    {
        _roomRepoMock = new Mock<IRoomRepository>();
        _useCase = new GetAvailableRooms(_roomRepoMock.Object);
    }

    [Fact]
    public void Execute_ShouldReturnAvailableRooms()
    {
        // Arrange
        var startDate = DateTime.Today;
        var endDate = DateTime.Today.AddDays(3);

        var rooms = new List<Room>
        {
            new Room { Id = Guid.NewGuid(), Number = "101", Capacity = 1 },
            new Room { Id = Guid.NewGuid(), Number = "102", Capacity = 2 }
        };

        _roomRepoMock.Setup(r => r.GetAvailableRooms(startDate, endDate)).Returns(rooms);

        // Act
        var result = _useCase.Execute(startDate, endDate);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, ((List<Room>)result).Count);
    }

    [Fact]
    public void Execute_ShouldReturnEmptyList_WhenNoRoomsAvailable()
    {
        var startDate = DateTime.Today;
        var endDate = DateTime.Today.AddDays(3);

        _roomRepoMock.Setup(r => r.GetAvailableRooms(startDate, endDate)).Returns(new List<Room>());

        var result = _useCase.Execute(startDate, endDate);

        Assert.Empty(result);
    }
}
