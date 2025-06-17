using GestionHotel.Domain.Interfaces;
using GestionHotel.Externals.PaiementGateways.Paypal;

namespace GestionHotel.Infrastructure.Repositories;

public class PaypalPaymentGatewayAdapter : IPaymentGateway
{
    private readonly PaypalGateway _paypalGateway;

    public PaypalPaymentGatewayAdapter()
    {
        _paypalGateway = new PaypalGateway();
    }

    public async Task<bool> ProcessPaymentAsync(string cardNumber, string expiryDate, decimal amount)
    {
        var result = await _paypalGateway.ProcessPaymentAsync(cardNumber, expiryDate, amount.ToString("F2", System.Globalization.CultureInfo.InvariantCulture));
        return result.Status == PaypalResultStatus.Success;
    }
}
