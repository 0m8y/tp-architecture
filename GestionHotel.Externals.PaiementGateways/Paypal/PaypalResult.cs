namespace GestionHotel.Externals.PaiementGateways.Paypal;

// Ne pas modifier ce fichier
public class PaypalResult
{
    public string TransactionId { get; set; }
    public PaypalResultStatus Status { get; set; }

    public static PaypalResult SuccessResult()
    {
        return new PaypalResult
        {
            TransactionId = Guid.NewGuid().ToString(),
            Status = PaypalResultStatus.Success
        };
    }
}