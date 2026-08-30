using System;
using System.Collections.Generic;
using System.Text;

namespace EventBooking.Application.booking.Dtos
{
    public class BookingResponse
    {
        public Guid BookingId { get; set; }
        public required string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime? HoldExpiresAtUtc { get; set; }
        public required List<BookedSeatDto> Seats { get; set; }
    }
    public class BookedSeatDto
    {
        public Guid EventSeatId { get; set; }
        public required string RowLabel { get; set; }
        public required string SeatNumber { get; set; }
        public decimal Price { get; set; }
    }
}
