namespace EventBooking.Application.Auth.DTOs
{
    public class AuthResponse
    {
        public required string Token { get; set; }
        public required string Email { get; set; }
        public required string FullName { get; set; }
        public required IList<string> Roles { get; set; }
    }
}