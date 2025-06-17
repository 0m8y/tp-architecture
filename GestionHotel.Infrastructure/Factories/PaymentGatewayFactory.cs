using GestionHotel.Application.Factory;
using GestionHotel.Domain.Enums;
using GestionHotel.Domain.Interfaces;
using GestionHotel.Infrastructure.Repositories;
namespace GestionHotel.Domain.Interfaces;

public class PaymentGatewayFactory : IPaymentGatewayFactory
{
    private readonly StripePaymentGatewayAdapter _stripe;
    private readonly PaypalPaymentGatewayAdapter _paypal;

    public PaymentGatewayFactory(
        StripePaymentGatewayAdapter stripe,
        PaypalPaymentGatewayAdapter paypal)
    {
        _stripe = stripe;
        _paypal = paypal;
    }

    public IPaymentGateway Get(PaymentProvider provider)
    {
        return provider switch
        {
            PaymentProvider.Stripe => _stripe,
            PaymentProvider.Paypal => _paypal,
            _ => throw new ArgumentException("Unknown provider")
        };
    }
}
