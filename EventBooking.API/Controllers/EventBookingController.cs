using EventBooking.Application.booking.Dtos;
using EventBooking.Application.Bookings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EventBooking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EventBookingController (IBookingService _bookingService) : ControllerBase
    {
        [HttpPost("hold-seats")]
        public async Task<IActionResult> HoldSeats(HoldSeatsRequest request)
        {
            var userId = GetCurrentUserId();
            var result = await _bookingService.HoldSeatsAsync(userId, request);
            return Ok(result);
        }
        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value;

            if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException("لا يمكن التحقق من هوية المستخدم");
            }

            return userId;
        }
    }
}
