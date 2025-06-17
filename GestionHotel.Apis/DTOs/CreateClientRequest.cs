using System.ComponentModel.DataAnnotations;

namespace GestionHotel.Apis.DTOs
{
    public class CreateClientRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress(ErrorMessage = "L'adresse e-mail n'est pas valide.")]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
