using EventBooking.Application.Locations;
using EventBooking.Application.Locations.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventBooking.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationsController(ILocationService locationService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var locations = await locationService.GetAllAsync();
            return Ok(locations);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<IActionResult> Create(CreateLocationRequest request)
        {
            var location = await locationService.CreateAsync(request);
            return Ok(location);
        }
    }
}