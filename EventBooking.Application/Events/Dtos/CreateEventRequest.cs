namespace EventBooking.Application.Events.DTOs
{
    public class CreateEventRequest
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime StartDateUtc { get; set; }
        public DateTime EndDateUtc { get; set; }
        public Guid HallId { get; set; }
        public Guid CategoryId { get; set; }
    }
}