using GestionHotel.Domain.Enums;
using GestionHotel.Domain.Interfaces;

namespace GestionHotel.Application.Factory;

public interface IPaymentGatewayFactory
{
    IPaymentGateway Get(PaymentProvider provider);
}
