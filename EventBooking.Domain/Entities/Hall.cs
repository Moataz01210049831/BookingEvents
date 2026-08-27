namespace EventBooking.Domain.Entities
{
    public class Hall
    {
        public Guid Id { get; set; }
        public Guid LocationId { get; set; }
        public required string Name { get; set; }

        public EventLocation Location { get; set; } = null!;
        public ICollection<Seat> Seats { get; set; } = new List<Seat>();
        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}