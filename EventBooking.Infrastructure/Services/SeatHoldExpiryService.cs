using EventBooking.Domain.Entities;
using EventBooking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EventBooking.Infrastructure.Services
{
    public class SeatHoldExpiryService(IServiceScopeFactory _scopeFactory) : BackgroundService
    {
        private static readonly TimeSpan CheckInterval = TimeSpan.FromMinutes(1);

       

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var now = DateTime.UtcNow;

                // 1. هات الكراسي المنتهية
                var expiredSeats = await context.EventSeats
                    .Where(es => es.Status == EventSeatStatus.Held && es.HoldExpiresAtUtc < now)
                    .ToListAsync(stoppingToken);

                if (expiredSeats.Count > 0)
                {
                    var expiredSeatIds = expiredSeats.Select(es => es.Id).ToList();

                    // 2. هات الـ BookingSeats المرتبطة بالكراسي دي
                    var bookingSeats = await context.BookingSeats
                        .Where(bs => expiredSeatIds.Contains(bs.EventSeatId))
                        .ToListAsync(stoppingToken);

                    var affectedBookingIds = bookingSeats.Select(bs => bs.BookingId).Distinct().ToList();

                    // 3. حدّث حالة الحجوزات المرتبطة إلى Expired (لو لسه Pending)
                    var bookingsToExpire = await context.Bookings
                        .Where(b => affectedBookingIds.Contains(b.Id) && b.Status == BookingStatus.Pending)
                        .ToListAsync(stoppingToken);

                    foreach (var booking in bookingsToExpire)
                    {
                        booking.Status = BookingStatus.Expired;
                    }

                    // 4. امسح صفوف BookingSeats القديمة (عشان نسمح بحجز الكرسي تاني)
                    context.BookingSeats.RemoveRange(bookingSeats);

                    // 5. رجّع الكراسي Available
                    foreach (var seat in expiredSeats)
                    {
                        seat.Status = EventSeatStatus.Available;
                        seat.HeldByUserId = null;
                        seat.HoldExpiresAtUtc = null;
                    }

                    await context.SaveChangesAsync(stoppingToken);
                }

                await Task.Delay(CheckInterval, stoppingToken);
            }
        }
    }
}