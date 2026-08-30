using EventBooking.Application.booking.Dtos;
using EventBooking.Application.Bookings.DTOs;

namespace EventBooking.Application.Bookings
{
    public interface IBookingService
    {
        Task<BookingResponse> HoldSeatsAsync(Guid userId, HoldSeatsRequest request);
        Task<BookingResponse> ConfirmBookingAsync(Guid userId, ConfirmBookingRequest request);
        Task<List<MyBookingDto>> GetMyBookingsAsync(Guid userId);

    }
}