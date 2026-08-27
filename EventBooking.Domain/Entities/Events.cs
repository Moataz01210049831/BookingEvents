namespace EventBooking.Domain.Entities
{
    public enum EventStatus
    {
        Draft = 0,
        Published = 1,
        Cancelled = 2,
        Completed = 3
    }

    public class Event
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime StartDateUtc { get; set; }
        public DateTime EndDateUtc { get; set; }
        public Guid HallId { get; set; }
        public Guid CategoryId { get; set; }
        public Guid OrganizerId { get; set; }
        public EventStatus Status { get; set; }
        public DateTime CreatedAtUtc { get; set; }

        // Navigation Properties
        public Hall Hall { get; set; } = null!;
        public EventCategory Category { get; set; } = null!;
        public ApplicationUser Organizer { get; set; } = null!;

        public ICollection<EventSeat> EventSeats { get; set; } = new List<EventSeat>();
    }
}