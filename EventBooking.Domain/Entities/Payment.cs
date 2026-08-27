namespace EventBooking.Domain.Entities
{
    public enum PaymentStatus
    {
        Pending = 0,
        Success = 1,
        Failed = 2,
        Refunded = 3
    }

    public class Payment
    {
        public Guid Id { get; set; }
        public Guid BookingId { get; set; }
        public string? TransactionReference { get; set; }
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTime CreatedAtUtc { get; set; }

        // Navigation Property
        public Booking Booking { get; set; } = null!;
    }
}