namespace EventBooking.Application.Halls.DTOs
{
    public class CreateSeatsRequest
    {
        public Guid HallId { get; set; }
        public required List<SeatInputDto> Seats { get; set; }
    }

    public class SeatInputDto
    {
        public required string RowLabel { get; set; }
        public required string SeatNumber { get; set; }
        public string SeatType { get; set; } = "Regular";
    }
}