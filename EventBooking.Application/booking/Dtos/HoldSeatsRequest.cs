using System;
using System.Collections.Generic;
using System.Text;

namespace EventBooking.Application.booking.Dtos
{
    public class HoldSeatsRequest
    {
        public required List<Guid> EventSeatIds { get; set; }
    }
}
