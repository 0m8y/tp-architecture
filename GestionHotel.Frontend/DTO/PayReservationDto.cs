using System.ComponentModel.DataAnnotations;

namespace GestionHotel.Frontend.Dto;

public class PayReservationDto
{
    [Required]
    [RegularExpression(@"^\d{16}$", ErrorMessage = "Le numéro doit contenir 16 chiffres.")]
    public string CardNumber { get; set; } = "";

    [Required]
    [RegularExpression(@"^(0[1-9]|1[0-2])\/\d{2}$", ErrorMessage = "Format attendu : MM/YY.")]
    public string ExpiryDate { get; set; } = "";

    [Required]
    public string Provider { get; set; } = "";
}