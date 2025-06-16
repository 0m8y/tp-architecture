using GestionHotel.Domain.Enums;

namespace GestionHotel.Domain.Interfaces;

public interface INotificationService
{
    Task SendAsync(string destination, string message, NotificationType type);
}