namespace GestionHotel.Externals.PaiementGateways.Stripe;

// Ne pas modifier ce fichier
public class StripeGateway
{
    public Task<bool> ProcessPaymentAsync(StripePayementInfo payementInfo)
    {
        Console.WriteLine("Processing payment with Stripe calling Stripe API");
        return Task.FromResult(true);
    }
}