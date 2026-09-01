namespace EventBooking.Application.Locations.DTOs
{
    public class CreateLocationRequest
    {
        public required string Name { get; set; }
        public required string Address { get; set; }
        public string? City { get; set; }
    }
}