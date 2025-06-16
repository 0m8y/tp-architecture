namespace GestionHotel.Externals.PaiementGateways.Paypal;

// Ne pas modifier ce fichier
public class PaypalGateway
{
    public Task<PaypalResult> ProcessPaymentAsync(string cardNumber, string expiryDate, string amount)
    {
        Console.WriteLine("Processing payment with Paypal calling Paypal API");
        return Task.FromResult(PaypalResult.SuccessResult());
    }
}