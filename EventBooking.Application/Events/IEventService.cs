using EventBooking.Application.Events.Dtos;
using EventBooking.Application.Events.DTOs;

namespace EventBooking.Application.Events
{
    public interface IEventService
    {
        Task<List<EventListItemDto>> GetAllEventsAsync();
        Task<EventDetailsDto> GetEventByIdAsync(Guid id);
    }
}