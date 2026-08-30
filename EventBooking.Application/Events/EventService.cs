using EventBooking.Application.Common.Exceptions;
using EventBooking.Application.Common.Interfaces;
using EventBooking.Application.Events.Dtos;
using EventBooking.Application.Events.DTOs;
using EventBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventBooking.Application.Events
{
    public class EventService(IApplicationDbContext _context) : IEventService
    {
        

        public async Task<List<EventListItemDto>> GetAllEventsAsync()
        {
            var events = await _context.Events
                .Where(e => e.Status == EventStatus.Published)
                .Select(e => new EventListItemDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    Description = e.Description,
                    ImageUrl = e.ImageUrl,
                    StartDateUtc = e.StartDateUtc,
                    EndDateUtc = e.EndDateUtc,
                    CategoryName = e.Category.Name,
                    HallName = e.Hall.Name,
                    LocationName = e.Hall.Location.Name,
                    AvailableSeatsCount = e.EventSeats.Count(es => es.Status == EventSeatStatus.Available)
                })
                .ToListAsync();

            return events;
        }

        public async Task<EventDetailsDto> GetEventByIdAsync(Guid id)
        {
            var eventEntity = await _context.Events
                .Where(e => e.Id == id)
                .Select(e => new EventDetailsDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    Description = e.Description,
                    ImageUrl = e.ImageUrl,
                    StartDateUtc = e.StartDateUtc,
                    EndDateUtc = e.EndDateUtc,
                    CategoryName = e.Category.Name,
                    HallName = e.Hall.Name,
                    LocationName = e.Hall.Location.Name,
                    Seats = e.EventSeats.Select(es => new SeatDto
                    {
                        EventSeatId = es.Id,
                        RowLabel = es.Seat.RowLabel,
                        SeatNumber = es.Seat.SeatNumber,
                        SeatType = es.Seat.SeatType.ToString(),
                        Price = es.Price,
                        Status = es.Status.ToString()
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (eventEntity is null)
            {
                throw new NotFoundException("الحدث المطلوب غير موجود");
            }

            return eventEntity;
        }
    }
}