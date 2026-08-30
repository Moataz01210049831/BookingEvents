using System;
using System.Collections.Generic;
using System.Text;

namespace EventBooking.Application.Events.Dtos
{
    public class SeatDto
    {
        public Guid EventSeatId { get; set; }
        public required string RowLabel { get; set; }
        public required string SeatNumber { get; set; }
        public required string SeatType { get; set; }
        public decimal Price { get; set; }
        public required string Status { get; set; }
    }
}
