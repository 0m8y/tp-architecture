using GestionHotel.Apis.DTOs;
using GestionHotel.Apis.Helpers;
using GestionHotel.Application.UseCases.Booking;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniValidation;

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

        group.MapPost("/available-rooms", (
            [FromBody] GetAvailableRoomsRequest request,
            HttpContext context,
            GetAvailableRooms useCase) =>
        {
            var rooms = useCase.Execute(request.StartDate, request.EndDate);

            var isReceptionist = context.User?.Identity?.IsAuthenticated == true
                                 && context.User.IsInRole("Receptionist");

            if (isReceptionist)
            {
                var detailed = rooms.Select(r => new AvailableRoomForReceptionistDto
                {
                    Id = r.Id,
                    Number = r.Number,
                    Capacity = r.Capacity,
                    Type = r.Type,
                    Condition = r.Condition
                });

                return Results.Ok(detailed);
            }
            else
            {
                var basic = rooms.Select(r => new AvailableRoomForClientDto
                {
                    Id = r.Id,
                    Number = r.Number,
                    Capacity = r.Capacity,
                    Type = r.Type,
                });

                return Results.Ok(basic);
            }
        })
        .WithName("GetAvailableRooms")
        .WithOpenApi();

        group.MapGet("/my-reservations", [Authorize] (
            HttpContext context,
            GetReservationsByClient useCase) =>
        {
            var clientId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (clientId == null)
                return Results.Unauthorized();

            var reservations = useCase.Execute(Guid.Parse(clientId));

            var dtos = reservations.Select(r => new ReservationDto
            {
                Id = r.Id,
                StartDate = r.StartDate,
                EndDate = r.EndDate,
                TotalAmount = r.TotalAmount,
                IsPaid = r.IsPaid,
                Status = r.Status.ToString(),
                RoomNumbers = r.ReservationRooms
                    .Select(rr => rr.Room.Number)
                    .Distinct()
                    .ToList()
            }).ToList();

            return Results.Ok(dtos);
        });

        group.MapPost("/reservations/{id:guid}/pay", [Authorize] async (
           Guid id,
           PayReservationRequest request,
           PayReservation useCase) =>
        {
            if (!MiniValidator.TryValidate(request, out var errors))
                return Results.ValidationProblem(errors);

            var result = await useCase.ExecuteAsync(id, request.CardNumber, request.ExpiryDate, request.Provider);

            if (!result.IsSuccess)
                return Results.BadRequest(result.ErrorMessage);

            return Results.Ok("Payment successful and reservation confirmed");
        });

        group.MapPost("/reservations/{id:guid}/cancel", [Authorize] (
            Guid id,
            HttpContext context,
            [FromServices] CancelReservation useCase) =>
        {
            var clientId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (clientId == null)
                return Results.Unauthorized();

            var result = useCase.Execute(id, Guid.Parse(clientId));
            return result.IsSuccess
                ? Results.Ok(result.Message)
                : Results.BadRequest(result.ErrorMessage);
        });
    }
}
