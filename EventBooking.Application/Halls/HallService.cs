using EventBooking.Application.Common.Exceptions;
using EventBooking.Application.Common.Interfaces;
using EventBooking.Application.Halls.DTOs;
using EventBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventBooking.Application.Halls
{
    public class HallService(IApplicationDbContext context, IMessageService messages) : IHallService
    {
        public async Task<List<HallDto>> GetAllAsync()
        {
            return await context.Halls
                .Select(h => new HallDto
                {
                    Id = h.Id,
                    Name = h.Name,
                    LocationId = h.LocationId,
                    LocationName = h.Location.Name
                })
                .ToListAsync();
        }

        public async Task<HallDto> CreateAsync(CreateHallRequest request)
        {
            var locationExists = await context.EventLocations
                .AnyAsync(l => l.Id == request.LocationId);

            if (!locationExists)
            {
                throw new NotFoundException(messages.Get("LocationNotFound"));
            }

            var hall = new Hall
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                LocationId = request.LocationId
            };

            context.Halls.Add(hall);
            await context.SaveChangesAsync();

            var location = await context.EventLocations.FirstAsync(l => l.Id == request.LocationId);

            return new HallDto
            {
                Id = hall.Id,
                Name = hall.Name,
                LocationId = hall.LocationId,
                LocationName = location.Name
            };
        }

        public async Task<int> AddSeatsAsync(CreateSeatsRequest request)
        {
            var hallExists = await context.Halls.AnyAsync(h => h.Id == request.HallId);
            if (!hallExists)
            {
                throw new NotFoundException(messages.Get("HallNotFound"));
            }

            var addedCount = 0;
            foreach (var seatInput in request.Seats)
            {
                var alreadyExists = await context.Seats.AnyAsync(s =>
                    s.HallId == request.HallId &&
                    s.RowLabel == seatInput.RowLabel &&
                    s.SeatNumber == seatInput.SeatNumber);

                if (alreadyExists)
                {
                    continue; // تخطى المقعد ده، already موجود
                }
                var seatType = Enum.TryParse<SeatType>(seatInput.SeatType, out var parsed)
                  ? parsed
                  : SeatType.Regular;

                context.Seats.Add(new Seat
                {
                    Id = Guid.NewGuid(),
                    HallId = request.HallId,
                    RowLabel = seatInput.RowLabel,
                    SeatNumber = seatInput.SeatNumber,
                    SeatType = seatType
                });

                addedCount++;
            }

            await context.SaveChangesAsync();

            return addedCount;
        }
    }
}