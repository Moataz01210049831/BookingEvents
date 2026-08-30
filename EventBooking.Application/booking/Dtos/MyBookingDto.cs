using EventBooking.Application.booking.Dtos;

namespace EventBooking.Application.Bookings.DTOs
{
    public class MyBookingDto
    {
        public Guid BookingId { get; set; }
        public required string EventTitle { get; set; }
        public DateTime EventStartDateUtc { get; set; }
        public required string HallName { get; set; }
        public required string LocationName { get; set; }
        public required string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime BookingDateUtc { get; set; }
        public required List<BookedSeatDto> Seats { get; set; }
    }
}