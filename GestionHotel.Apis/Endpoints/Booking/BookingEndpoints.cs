namespace GestionHotel.Apis.Endpoints.Booking;


public static class BookingEndpoints
{
    private const string BASE_URL = "/api/v1/booking/";

    public static void MapBookingsEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup(BASE_URL)
            .WithOpenApi()
            .WithTags("Booking");

        group.MapGet("", BookingHandler.GetAvailableRooms)
            .WithName("GetAvailableRooms");

        group.MapPost("", BookingHandler.Create)
            .WithName("CreateBooking");
    }
}