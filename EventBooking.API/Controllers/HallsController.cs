using EventBooking.Application.Halls;
using EventBooking.Application.Halls.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventBooking.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HallsController(IHallService hallService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var halls = await hallService.GetAllAsync();
            return Ok(halls);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<IActionResult> Create(CreateHallRequest request)
        {
            var hall = await hallService.CreateAsync(request);
            return Ok(hall);
        }

        [HttpPost("seats")]
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<IActionResult> AddSeats(CreateSeatsRequest request)
        {
            var addedCount = await hallService.AddSeatsAsync(request);
            return Ok(new { addedCount });
        }
    }
}