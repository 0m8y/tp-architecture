namespace GestionHotel.Application.Services;

public static class SessionStore
{
    public static Dictionary<string, Guid> Tokens = new();
}
