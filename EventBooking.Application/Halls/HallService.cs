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
    }
}