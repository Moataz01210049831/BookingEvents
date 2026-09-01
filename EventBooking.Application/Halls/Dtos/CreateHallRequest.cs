namespace EventBooking.Application.Halls.DTOs
{
    public class CreateHallRequest
    {
        public required string Name { get; set; }
        public Guid LocationId { get; set; }
    }
}