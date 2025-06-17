using GestionHotel.Domain.Entities;
using GestionHotel.Domain.Interfaces;

namespace GestionHotel.Application.UseCases.Booking;

public class GetReservationsByClient
{
    private readonly IReservationRepository _reservationRepository;

    public GetReservationsByClient(IReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    public List<Reservation> Execute(Guid clientId)
    {
        return _reservationRepository.GetByClientId(clientId);
    }
}
