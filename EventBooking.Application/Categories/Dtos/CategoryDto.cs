namespace EventBooking.Application.Categories.DTOs
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
    }
}