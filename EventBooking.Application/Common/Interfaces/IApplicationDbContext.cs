using EventBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventBooking.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<EventCategory> EventCategories { get; }
        DbSet<EventLocation> EventLocations { get; }
        DbSet<Hall> Halls { get; }
        DbSet<Seat> Seats { get; }
        DbSet<Event> Events { get; }
        DbSet<EventSeat> EventSeats { get; }
        DbSet<Booking> Bookings { get; }
        DbSet<BookingSeat> BookingSeats { get; }
        DbSet<Payment> Payments { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}