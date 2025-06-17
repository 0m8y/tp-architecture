using GestionHotel.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class PayReservationRequest
{
    [Required]
    [RegularExpression(@"^\d{16}$", ErrorMessage = "Card number must be exactly 16 digits.")]
    public string CardNumber { get; set; } = "";

    [Required]
    [RegularExpression(@"^(0[1-9]|1[0-2])\/\d{2}$", ErrorMessage = "Expiry date must be in MM/YY format.")]
    public string ExpiryDate { get; set; } = "";

    [JsonConverter(typeof(JsonStringEnumConverter))]
    [Required]
    public PaymentProvider Provider { get; set; }
}
