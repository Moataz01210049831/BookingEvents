using System.ComponentModel.DataAnnotations.Schema;

namespace EventBooking.Domain.Entities
{
    [Table("EventLocation")]
    public class EventLocation
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Address { get; set; }
        public string? City { get; set; }

        public ICollection<Hall> Halls { get; set; } = new List<Hall>();
    }
}