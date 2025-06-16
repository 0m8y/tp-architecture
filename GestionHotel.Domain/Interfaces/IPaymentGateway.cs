namespace GestionHotel.Domain.Interfaces;

public interface IPaymentGateway
{
    Task<bool> ProcessPaymentAsync(string cardNumber, string expiryDate, decimal amount);
}
