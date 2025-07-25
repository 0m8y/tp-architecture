﻿using GestionHotel.Domain.Enums;

namespace GestionHotel.Domain.Entities;

public class Room
{
    public Guid Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public RoomType Type { get; set; }
    public int Capacity { get; set; }
    public RoomCondition Condition { get; set; }
    public List<ReservationRoom> ReservationRooms { get; set; } = new();
    public bool NeedsCleaning { get; set; }
}
