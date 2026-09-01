using EventBooking.Application.Locations.DTOs;

namespace EventBooking.Application.Locations
{
    public interface ILocationService
    {
        Task<List<LocationDto>> GetAllAsync();
        Task<LocationDto> CreateAsync(CreateLocationRequest request);
    }
}