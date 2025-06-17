using GestionHotel.Domain.Interfaces;
using GestionHotel.Externals.PaiementGateways.Stripe;

namespace GestionHotel.Infrastructure.PaiementGateways;

public class StripePaymentGatewayAdapter : IPaymentGateway
{
    private readonly StripeGateway _stripeGateway;

    public StripePaymentGatewayAdapter()
    {
        _stripeGateway = new StripeGateway();
    }

    public async Task<bool> ProcessPaymentAsync(string cardNumber, string expiryDate, decimal amount)
    {
        var stripeInfo = new StripePayementInfo
        {
            CardNumber = cardNumber,
            ExpiryDate = expiryDate,
            Amount = amount.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)
        };

        return await _stripeGateway.ProcessPaymentAsync(stripeInfo);
    }
}
