namespace GestionHotel.Externals.PaiementGateways.Stripe;

// Ne pas modifier ce fichier
public class StripePayementInfo
{
    public string CardNumber { get; set; }
    public string ExpiryDate { get; set; }
    public string Amount { get; set; }
}