namespace EventBooking.Application.Events.DTOs
{
    public class EventListItemDto
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime StartDateUtc { get; set; }
        public DateTime EndDateUtc { get; set; }
        public required string CategoryName { get; set; }
        public required string HallName { get; set; }
        public required string LocationName { get; set; }
        public int AvailableSeatsCount { get; set; }
    }
}