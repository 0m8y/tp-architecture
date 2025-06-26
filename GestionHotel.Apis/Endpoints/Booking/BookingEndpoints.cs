using GestionHotel.Apis.DTOs;
using GestionHotel.Apis.Helpers;
using GestionHotel.Application.UseCases.Booking;
using GestionHotel.Domain.Enums;
using GestionHotel.Domain.Interfaces;
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
                var result = useCase.Execute(clientId, request.StartDate, request.EndDate, request.RoomIds);

                if (!result.IsSuccess)
                    return Results.BadRequest(result.ErrorMessage);

                var reservation = result.Value!;
                var dto = new ReservationDto
                {
                    Id = reservation.Id,
                    StartDate = reservation.StartDate,
                    EndDate = reservation.EndDate,
                    TotalAmount = reservation.TotalAmount,
                    IsPaid = reservation.IsPaid,
                    Status = reservation.Status.ToString(),
                    RoomIds = reservation.ReservationRooms
                        .Select(rr => rr.RoomId)
                        .Distinct()
                        .ToList()
                };

                return Results.Ok(dto);

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
                RoomIds = r.ReservationRooms
                    .Select(rr => rr.RoomId)
                    .Distinct()
                    .ToList()
            }).ToList();

            return Results.Ok(dtos);
        });

        group.MapGet("/reservations/{id:guid}", [Authorize] async (
            Guid id,
            HttpContext context,
            IReservationRepository reservationRepository) =>
        {
            var userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var role = context.User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            if (userId is null || role is null)
                return Results.Unauthorized();

            var reservation = reservationRepository.GetByReservationId(id);
            if (reservation is null)
                return Results.NotFound();

            if (role != "Receptionist" && reservation.ClientId.ToString() != userId)
                return Results.Forbid();

            var dto = new ReservationDto
            {
                Id = reservation.Id,
                StartDate = reservation.StartDate,
                EndDate = reservation.EndDate,
                TotalAmount = reservation.TotalAmount,
                IsPaid = reservation.IsPaid,
                Status = reservation.Status.ToString(),
                RoomIds = reservation.ReservationRooms
                    .Select(r => r.RoomId)
                    .ToList()
            };

            return Results.Ok(dto);
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
            [FromServices] CancelReservation useCase,
            [FromQuery] bool refund = false
            ) =>
        {
            var user = context.User;
            var clientIdClaim = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var isReceptionist = user.IsInRole("Receptionist");

            if (!isReceptionist && clientIdClaim == null)
                return Results.Unauthorized();

            var clientId = isReceptionist ? Guid.Empty : Guid.Parse(clientIdClaim);

            var result = useCase.Execute(
                reservationId: id,
                clientId: clientId,
                isReceptionist: isReceptionist,
                forceRefund: isReceptionist && refund
            );

            return result.IsSuccess
                ? Results.Ok(result.Message)
                : Results.BadRequest(result.ErrorMessage);
        });

        group.MapPost("/reservations/{id:guid}/checkin", [Authorize(Roles = "Receptionist")] async (
            Guid id,
            CheckInReservation useCase,
            [FromBody] CheckInReservationRequest request = null) =>
        {
            if (request != null && !MiniValidator.TryValidate(request, out var errors))
                return Results.ValidationProblem(errors);
            
            var result = await useCase.ExecuteAsync(
                id,
                request?.CardNumber,
                request?.ExpiryDate,
                request?.Provider
            );

            if (!result.IsSuccess)
                return Results.BadRequest(result.ErrorMessage);

            return Results.Ok(result.Message);
        })
        .WithName("CheckInReservation")
        .WithOpenApi();

        group.MapGet("/all-reservations", [Authorize(Roles = "Receptionist")] (
            IReservationRepository reservationRepository) =>
        {
            var reservations = reservationRepository.GetAll();

            var dtos = reservations.Select(r => new ReservationDto
            {
                Id = r.Id,
                StartDate = r.StartDate,
                EndDate = r.EndDate,
                TotalAmount = r.TotalAmount,
                IsPaid = r.IsPaid,
                Status = r.Status.ToString(),
                RoomIds = r.ReservationRooms
                    .Select(rr => rr.RoomId)
                    .Distinct()
                    .ToList()
            }).ToList();

            return Results.Ok(dtos);
        });

        group.MapPost("/reservations/{id:guid}/checkout", [Authorize(Roles = "Receptionist")] async (
            Guid id,
            [FromServices] CheckOutReservation useCase) =>
        {
            var result = useCase.Execute(id);

            return result.IsSuccess
                ? Results.Ok(result.Message)
                : Results.BadRequest(result.ErrorMessage);
        });

        group.MapGet("/rooms-to-clean", [Authorize(Roles = "Cleaner")] (
            GetRoomsToClean useCase) =>
        {
            var result = useCase.Execute();

            var dtos = result.Select(r => new RoomToCleanDto
            {
                RoomId = r.RoomId,
                RoomNumber = r.RoomNumber,
                LastOccupied = r.LastOccupied,
                NextOccupied = r.NextOccupied
            });

            return Results.Ok(dtos);
        })
        .WithName("GetRoomsToClean")
        .WithOpenApi();

        group.MapPost("/rooms/cleaned", [Authorize(Roles = "Cleaner")] (
            [FromBody] MarkRoomAsCleanedRequest request,
            MarkRoomAsCleaned useCase) =>
        {
            var result = useCase.Execute(request.RoomId);

            return result.IsSuccess
                ? Results.Ok(result.Message)
                : Results.BadRequest(result.ErrorMessage);
        })
        .WithName("MarkRoomAsCleaned")
        .WithOpenApi();
    }
}
