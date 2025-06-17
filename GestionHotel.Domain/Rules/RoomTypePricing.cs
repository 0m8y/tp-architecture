using GestionHotel.Domain.Enums;

namespace GestionHotel.Domain.Rules;

public static class RoomTypePricing
{
    public static decimal GetPrice(RoomType type) => type switch
    {
        RoomType.Single => 80m,
        RoomType.Double => 120m,
        RoomType.Suite => 250m,
        _ => throw new ArgumentOutOfRangeException(nameof(type), $"Tarif inconnu pour le type {type}")
    };
}
