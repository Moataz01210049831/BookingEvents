using System;
using System.Collections.Generic;
using System.Text;

namespace EventBooking.Domain.Entities
{
    public class EventCategory
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }

        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}
