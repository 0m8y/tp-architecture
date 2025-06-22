namespace GestionHotel.Frontend.Dto;

public class ReservationDto
{
    public Guid Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalAmount { get; set; }
    public bool IsPaid { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<Guid> RoomIds { get; set; } = new();

}

