namespace GestionHotel.Domain.Entities;

public class Client
{
    public Guid Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
