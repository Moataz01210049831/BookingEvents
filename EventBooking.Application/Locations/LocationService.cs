using EventBooking.Application.Common.Interfaces;
using EventBooking.Application.Locations.DTOs;
using EventBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventBooking.Application.Locations
{
    public class LocationService(IApplicationDbContext context) : ILocationService
    {
        public async Task<List<LocationDto>> GetAllAsync()
        {
            return await context.EventLocations
                .Select(l => new LocationDto
                {
                    Id = l.Id,
                    Name = l.Name,
                    Address = l.Address,
                    City = l.City
                })
                .ToListAsync();
        }

        public async Task<LocationDto> CreateAsync(CreateLocationRequest request)
        {
            var location = new EventLocation
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Address = request.Address,
                City = request.City
            };

            context.EventLocations.Add(location);
            await context.SaveChangesAsync();

            return new LocationDto
            {
                Id = location.Id,
                Name = location.Name,
                Address = location.Address,
                City = location.City
            };
        }
    }
}