using GestionHotel.Application.UseCases.Booking;
using GestionHotel.Domain.Interfaces;
using GestionHotel.Apis.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace GestionHotel.Apis.Endpoints.Booking;

public static class BookingEndpoints
{
    public static void MapBookingsEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/v1/bookings")
            .WithTags("Booking");

        group.MapPost("/available-rooms", async (
            [FromBody] GetAvailableRoomsRequest request,
            [FromServices] GetAvailableRooms useCase) =>
        {
            var rooms = useCase.Execute(request.StartDate, request.EndDate);
            return Results.Ok(rooms);
        })
        .WithName("GetAvailableRooms")
        .WithOpenApi();
    }
}
