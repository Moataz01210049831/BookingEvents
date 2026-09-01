using EventBooking.Application.Halls.DTOs;

namespace EventBooking.Application.Halls
{
    public interface IHallService
    {
        Task<List<HallDto>> GetAllAsync();
        Task<HallDto> CreateAsync(CreateHallRequest request);
    }
}