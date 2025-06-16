using GestionHotel.Domain.Enums;

namespace GestionHotel.Domain.Entities;

public class Employe
{
    public Guid Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public RoleEmploye Role { get; set; }
}
