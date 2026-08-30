using EventBooking.Application.booking.Dtos;

namespace EventBooking.Application.Bookings
{
    public interface IBookingService
    {
        Task<BookingResponse> HoldSeatsAsync(Guid userId, HoldSeatsRequest request);
    }
}