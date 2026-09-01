using EventBooking.Application.booking.Dtos;
using EventBooking.Application.Bookings;
using EventBooking.Application.Bookings.DTOs;
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

        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmBooking(ConfirmBookingRequest request)
        {
            var userId = GetCurrentUserId();
            var result = await _bookingService.ConfirmBookingAsync(userId, request);
            return Ok(result);
        }

        [HttpGet("my-bookings")]
        public async Task<IActionResult> GetMyBookings()
        {
            var userId = GetCurrentUserId();
            var bookings = await _bookingService.GetMyBookingsAsync(userId);
            return Ok(bookings);
        }

        [HttpGet("api/debug/my-claims")]
        [Authorize]
        public IActionResult GetMyClaims()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            return Ok(claims);
        }


    }

}
