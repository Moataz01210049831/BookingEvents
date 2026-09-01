namespace EventBooking.Application.Halls.DTOs
{
    public class HallDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public Guid LocationId { get; set; }
        public required string LocationName { get; set; }
    }
}