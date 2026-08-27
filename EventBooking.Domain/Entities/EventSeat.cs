using System.ComponentModel.DataAnnotations;

namespace EventBooking.Domain.Entities
{
    public enum EventSeatStatus
    {
        Available = 0,
        Held = 1,
        Booked = 2,
        Blocked = 3
    }

    public class EventSeat
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public Guid SeatId { get; set; }
        public decimal Price { get; set; }
        public EventSeatStatus Status { get; set; }
        public Guid? HeldByUserId { get; set; }
        public DateTime? HoldExpiresAtUtc { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } = null!;

        // Navigation Properties
        public Event Event { get; set; } = null!;
        public Seat Seat { get; set; } = null!;
        public ApplicationUser? HeldByUser { get; set; }
        public BookingSeat? BookingSeat { get; set; }
    }
}