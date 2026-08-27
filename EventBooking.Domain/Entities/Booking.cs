namespace EventBooking.Domain.Entities
{
    public enum BookingStatus
    {
        Pending = 0,
        Confirmed = 1,
        Cancelled = 2,
        Expired = 3
    }

    public class Booking
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid EventId { get; set; }
        public DateTime BookingDateUtc { get; set; }
        public BookingStatus Status { get; set; }
        public decimal TotalAmount { get; set; }

        // Navigation Properties
        public ApplicationUser User { get; set; } = null!;
        public Event Event { get; set; } = null!;
        public ICollection<BookingSeat> BookingSeats { get; set; } = new List<BookingSeat>();
        public Payment? Payment { get; set; }
    }
}