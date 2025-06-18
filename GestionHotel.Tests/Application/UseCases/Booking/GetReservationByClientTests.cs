using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using GestionHotel.Domain.Entities;
using GestionHotel.Domain.Interfaces;
using GestionHotel.Application.UseCases.Booking;

namespace GestionHotel.Tests.UseCases.Booking;

public class GetReservationsByClientTests
{
    private readonly Mock<IReservationRepository> _reservationRepoMock;
    private readonly GetReservationsByClient _useCase;

    public GetReservationsByClientTests()
    {
        _reservationRepoMock = new Mock<IReservationRepository>();
        _useCase = new GetReservationsByClient(_reservationRepoMock.Object);
    }

    [Fact]
    public void Execute_ShouldReturnReservations_ForGivenClient()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var reservations = new List<Reservation>
        {
            new Reservation { Id = Guid.NewGuid(), ClientId = clientId },
            new Reservation { Id = Guid.NewGuid(), ClientId = clientId }
        };

        _reservationRepoMock.Setup(r => r.GetByClientId(clientId)).Returns(reservations);

        // Act
        var result = _useCase.Execute(clientId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void Execute_ShouldReturnEmptyList_WhenClientHasNoReservations()
    {
        var clientId = Guid.NewGuid();

        _reservationRepoMock.Setup(r => r.GetByClientId(clientId)).Returns(new List<Reservation>());

        var result = _useCase.Execute(clientId);

        Assert.Empty(result);
    }
}
