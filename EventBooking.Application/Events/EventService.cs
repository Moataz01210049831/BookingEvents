using EventBooking.Application.Common.Exceptions;
using EventBooking.Application.Common.Interfaces;
using EventBooking.Application.Events.Dtos;
using EventBooking.Application.Events.DTOs;
using EventBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventBooking.Application.Events
{
    public class EventService(IApplicationDbContext context, IMessageService messages) : IEventService
    {
        public async Task<List<EventListItemDto>> GetAllEventsAsync()
        {
            var events = await context.Events
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
            var eventEntity = await context.Events
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
                throw new NotFoundException(messages.Get("EventNotFound"));
            }

            return eventEntity;
        }

        public async Task<EventDetailsDto> CreateAsync(Guid organizerId, CreateEventRequest request)
        {
            if (request.EndDateUtc <= request.StartDateUtc)
            {
                throw new ValidationException(messages.Get("InvalidEventDates"));
            }

            var hall = await context.Halls
                .Include(h => h.Location)
                .Include(h => h.Seats)
                .FirstOrDefaultAsync(h => h.Id == request.HallId);

            if (hall is null)
            {
                throw new NotFoundException(messages.Get("HallNotFound"));
            }

            var categoryExists = await context.EventCategories
                .AnyAsync(c => c.Id == request.CategoryId);

            if (!categoryExists)
            {
                throw new NotFoundException(messages.Get("CategoryNotFound"));
            }

            var newEvent = new Event
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                ImageUrl = request.ImageUrl,
                StartDateUtc = request.StartDateUtc,
                EndDateUtc = request.EndDateUtc,
                HallId = request.HallId,
                CategoryId = request.CategoryId,
                OrganizerId = organizerId,
                Status = EventStatus.Published,
                CreatedAtUtc = DateTime.UtcNow
            };

            context.Events.Add(newEvent);

            // كل كرسي في القاعة ياخد EventSeat تلقائيًا، بسعر افتراضي حسب نوعه
            foreach (var seat in hall.Seats)
            {
                context.EventSeats.Add(new EventSeat
                {
                    Id = Guid.NewGuid(),
                    EventId = newEvent.Id,
                    SeatId = seat.Id,
                    Price = seat.SeatType == SeatType.VIP ? 200m : 100m,
                    Status = EventSeatStatus.Available
                });
            }

            await context.SaveChangesAsync();

            return await GetEventByIdAsync(newEvent.Id);
        }
    }
}