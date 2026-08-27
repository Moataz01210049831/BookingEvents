namespace EventBooking.Domain.Entities
{
    public class BookingSeat
    {
        public Guid Id { get; set; }
        public Guid BookingId { get; set; }
        public Guid EventSeatId { get; set; }
        public decimal PriceAtBooking { get; set; }

        // Navigation Properties
        public Booking Booking { get; set; } = null!;
        public EventSeat EventSeat { get; set; } = null!;
    }
}