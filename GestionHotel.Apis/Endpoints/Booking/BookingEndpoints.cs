using GestionHotel.Application.UseCases.Booking;
using GestionHotel.Domain.Interfaces;
using GestionHotel.Apis.DTOs;
using GestionHotel.Apis.Helpers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GestionHotel.Apis.Endpoints.Booking;

public static class BookingEndpoints
{
    public static void MapBookingsEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/v1/bookings")
            .WithTags("Booking");

        group.MapPost("/booking",
            [Authorize] async (
                CreateReservationRequest request,
                HttpContext context,
                CreateReservation useCase) =>
            {
                var clientId = context.GetClientIdFromToken();
                useCase.Execute(clientId, request.StartDate, request.EndDate, request.RoomIds);
                return Results.Ok("Réservation effectuée avec succès.");
            });

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
