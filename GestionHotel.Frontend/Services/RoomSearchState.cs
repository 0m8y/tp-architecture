using GestionHotel.Frontend.Dto;

public class RoomSearchState
{
    public List<RoomDto> AvailableRooms { get; private set; } = new();

    public void SetRooms(List<RoomDto> rooms)
    {
        AvailableRooms = rooms;
    }
}
