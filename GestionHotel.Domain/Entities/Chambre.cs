using GestionHotel.Domain.Enums;

namespace GestionHotel.Domain.Entities;

public class Chambre
{
    public Guid Id { get; set; }
    public string Numero { get; set; } = string.Empty;
    public TypeChambre Type { get; set; }
    public decimal Tarif { get; set; }
    public int Capacite { get; set; }
    public EtatChambre Etat { get; set; }
}

 