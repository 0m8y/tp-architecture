using GestionHotel.Domain.Enums;

namespace GestionHotel.Apis.DTOs;

public class AvailableRoomForClientDto
{
    public Guid Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public RoomType Type { get; set; }
    public int Capacity { get; set; }
}

public class AvailableRoomForReceptionistDto : AvailableRoomForClientDto
{
    public RoomCondition Condition { get; set; }
}
