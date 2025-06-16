using Microsoft.AspNetCore.Mvc;

namespace GestionHotel.Apis.Endpoints.Booking;

public static class BookingHandler
{
    public static Task<BookingView> GetAvailableRooms(HttpContext context, SampleInjectionInterface sampleInjectionInterface, [AsParameters] GetAvailableRoomsInput input)
    {
        sampleInjectionInterface.DoSomething();
        return Task.FromResult(new BookingView());
    }

    public static Task<BookingResult> Create(HttpContext context, [FromBody]BookingInput input)
    {
        return Task.FromResult(new BookingResult());
    }
}

public interface SampleInjectionInterface
{
    void DoSomething();
}

public class SampleInjectionImplementation : SampleInjectionInterface
{
    public void DoSomething()
    {
        Console.WriteLine("Do something");
    }
}