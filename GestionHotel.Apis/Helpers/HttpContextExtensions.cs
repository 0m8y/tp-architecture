using System.Security.Claims;

namespace GestionHotel.Apis.Helpers;

public static class HttpContextExtensions
{
    public static Guid GetClientIdFromToken(this HttpContext context)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userId, out var id) ? id : throw new UnauthorizedAccessException("ClientId non valide.");
    }
}
