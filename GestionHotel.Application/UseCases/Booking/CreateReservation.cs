using GestionHotel.Application.Common;
using GestionHotel.Domain.Entities;
using GestionHotel.Domain.Enums;
using GestionHotel.Domain.Interfaces;
using GestionHotel.Domain.Rules;

namespace GestionHotel.Application.UseCases.Booking;

public class CreateReservation
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IRoomRepository _roomRepository;

    public CreateReservation(IReservationRepository reservationRepository, IRoomRepository roomRepository)
    {
        _reservationRepository = reservationRepository;
        _roomRepository = roomRepository;
    }

    public Result<Reservation> Execute(Guid clientId, DateTime startDate, DateTime endDate, List<Guid> roomIds)
    {
        var rooms = roomIds
            .Select(id => _roomRepository.GetWithReservationsById(id))
            .Where(r => r != null)
            .ToList();

        if (rooms.Count != roomIds.Count)
            return Result<Reservation>.Failure("Une ou plusieurs chambres sont introuvables.");

        foreach (var room in rooms)
        {
            var overlapping = room.ReservationRooms.Any(rr =>
                rr.Reservation.Status != ReservationStatus.Cancelled &&
                startDate < rr.Reservation.EndDate &&
                endDate > rr.Reservation.StartDate);

            if (overlapping)
                return Result<Reservation>.Failure($"La chambre {room.Number} est déjà réservée.");
        }

        var totalAmount = rooms.Sum(r => RoomTypePricing.GetPrice(r.Type));

        var reservation = new Reservation
        {
            Id = Guid.NewGuid(),
            ClientId = clientId,
            StartDate = startDate,
            EndDate = endDate,
            TotalAmount = totalAmount,
            IsPaid = false,
            Status = ReservationStatus.PendingPayment,
            ReservationRooms = rooms.Select(r => new ReservationRoom
            {
                RoomId = r.Id
            }).ToList()
        };

        _reservationRepository.Create(reservation);
        return Result<Reservation>.Success(reservation);
    }
}
