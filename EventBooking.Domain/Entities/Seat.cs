namespace EventBooking.Domain.Entities
{
    public enum SeatType:byte
    {
        Regular = 0,
        VIP = 1,
        Accessible = 2
    }

    public class Seat
    {
        public Guid Id { get; set; }
        public Guid HallId { get; set; }
        public required string RowLabel { get; set; }
        public required string SeatNumber { get; set; }
        public SeatType SeatType { get; set; }

        public Hall Hall { get; set; } = null!;
        public ICollection<EventSeat> EventSeats { get; set; } = new List<EventSeat>();
    }
}