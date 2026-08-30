using EventBooking.Application.booking.Dtos;
using EventBooking.Application.Bookings.DTOs;
using EventBooking.Application.Common.Exceptions;
using EventBooking.Application.Common.Interfaces;
using EventBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventBooking.Application.Bookings
{
    public class BookingService : IBookingService
    {
        private readonly IApplicationDbContext _context;
        private static readonly TimeSpan HoldDuration = TimeSpan.FromMinutes(10);

        public BookingService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BookingResponse> HoldSeatsAsync(Guid userId, HoldSeatsRequest request)
        {
            if (request.EventSeatIds is null || request.EventSeatIds.Count == 0)
            {
                throw new ValidationException("لازم تختار مقعد واحد على الأقل");
            }

            // 1. هات الكراسي المطلوبة مع بيانات الـ Seat بتاعتها
            var eventSeats = await _context.EventSeats
                .Include(es => es.Seat)
                .Where(es => request.EventSeatIds.Contains(es.Id))
                .ToListAsync();

            // 2. تأكد إن كل الكراسي المطلوبة فعلاً موجودة
            if (eventSeats.Count != request.EventSeatIds.Count)
            {
                throw new ValidationException("بعض المقاعد المطلوبة غير موجودة");
            }

            // 3. تأكد إن كل الكراسي لسه Available
            var unavailableSeats = eventSeats.Where(es => es.Status != EventSeatStatus.Available).ToList();
            if (unavailableSeats.Count > 0)
            {
                throw new ValidationException("بعض المقاعد المختارة لم تعد متاحة، من فضلك اختر مقاعد أخرى");
            }

            // 4. تأكد إن كل الكراسي تابعة لنفس الـ Event
            var distinctEventIds = eventSeats.Select(es => es.EventId).Distinct().ToList();
            if (distinctEventIds.Count != 1)
            {
                throw new ValidationException("لا يمكن حجز مقاعد من فعاليات مختلفة في نفس العملية");
            }

            var eventId = distinctEventIds.First();
            var holdExpiresAt = DateTime.UtcNow.Add(HoldDuration);

            // 5. حدّث حالة كل كرسي إلى Held
            foreach (var eventSeat in eventSeats)
            {
                eventSeat.Status = EventSeatStatus.Held;
                eventSeat.HeldByUserId = userId;
                eventSeat.HoldExpiresAtUtc = holdExpiresAt;
            }

            // 6. اعمل الـ Booking
            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                EventId = eventId,
                BookingDateUtc = DateTime.UtcNow,
                Status = BookingStatus.Pending,
                TotalAmount = eventSeats.Sum(es => es.Price)
            };

            _context.Bookings.Add(booking);

            // 7. اربط كل كرسي بالـ Booking
            foreach (var eventSeat in eventSeats)
            {
                _context.BookingSeats.Add(new BookingSeat
                {
                    Id = Guid.NewGuid(),
                    BookingId = booking.Id,
                    EventSeatId = eventSeat.Id,
                    PriceAtBooking = eventSeat.Price
                });
            }

            // 8. احفظ كل حاجة - لو حصل تعارض (Concurrency)، هيرمي Exception هنا
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new ValidationException("للأسف بعض المقاعد تم حجزها للتو من مستخدم آخر، حاول تاني");
            }

            return new BookingResponse
            {
                BookingId = booking.Id,
                Status = booking.Status.ToString(),
                TotalAmount = booking.TotalAmount,
                HoldExpiresAtUtc = holdExpiresAt,
                Seats = eventSeats.Select(es => new BookedSeatDto
                {
                    EventSeatId = es.Id,
                    RowLabel = es.Seat.RowLabel,
                    SeatNumber = es.Seat.SeatNumber,
                    Price = es.Price
                }).ToList()
            };
        }
    

    public async Task<BookingResponse> ConfirmBookingAsync(Guid userId, ConfirmBookingRequest request)
        {
            // 1. هات الحجز مع الكراسي المرتبطة بيه
            var booking = await _context.Bookings
                .Include(b => b.BookingSeats)
                    .ThenInclude(bs => bs.EventSeat)
                        .ThenInclude(es => es.Seat)
                .FirstOrDefaultAsync(b => b.Id == request.BookingId);

            // 2. تأكد إن الحجز موجود
            if (booking is null)
            {
                throw new NotFoundException("الحجز المطلوب غير موجود");
            }

            // 3. تأكد إن الحجز ده بتاع نفس اليوزر
            if (booking.UserId != userId)
            {
                throw new ForbiddenException("لا يمكنك تأكيد حجز لا يخصك");
            }

            // 4. تأكد إن الحجز لسه Pending
            if (booking.Status != BookingStatus.Pending)
            {
                throw new ValidationException("لا يمكن تأكيد هذا الحجز، حالته الحالية: " + booking.Status);
            }

            // 5. تأكد إن كل الكراسي لسه Held بنفس الوقت المسموح
            var now = DateTime.UtcNow;
            var invalidSeats = booking.BookingSeats
                .Where(bs => bs.EventSeat.Status != EventSeatStatus.Held
                          || bs.EventSeat.HeldByUserId != userId
                          || bs.EventSeat.HoldExpiresAtUtc < now)
                .ToList();

            if (invalidSeats.Count > 0)
            {
                throw new ValidationException("انتهت مهلة حجز بعض المقاعد، من فضلك أعد المحاولة");
            }

            // 6. أكّد الحجز والكراسي
            booking.Status = BookingStatus.Confirmed;

            foreach (var bookingSeat in booking.BookingSeats)
            {
                bookingSeat.EventSeat.Status = EventSeatStatus.Booked;
                bookingSeat.EventSeat.HeldByUserId = null;
                bookingSeat.EventSeat.HoldExpiresAtUtc = null;
            }

            await _context.SaveChangesAsync();

            return new BookingResponse
            {
                BookingId = booking.Id,
                Status = booking.Status.ToString(),
                TotalAmount = booking.TotalAmount,
                HoldExpiresAtUtc = null,
                Seats = booking.BookingSeats.Select(bs => new BookedSeatDto
                {
                    EventSeatId = bs.EventSeatId,
                    RowLabel = bs.EventSeat.Seat.RowLabel,
                    SeatNumber = bs.EventSeat.Seat.SeatNumber,
                    Price = bs.PriceAtBooking
                }).ToList()
            };
        }

        public async Task<List<MyBookingDto>> GetMyBookingsAsync(Guid userId)
        {
            var bookings = await _context.Bookings
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.BookingDateUtc)
                .Select(b => new MyBookingDto
                {
                    BookingId = b.Id,
                    EventTitle = b.Event.Title,
                    EventStartDateUtc = b.Event.StartDateUtc,
                    HallName = b.Event.Hall.Name,
                    LocationName = b.Event.Hall.Location.Name,
                    Status = b.Status.ToString(),
                    TotalAmount = b.TotalAmount,
                    BookingDateUtc = b.BookingDateUtc,
                    Seats = b.BookingSeats.Select(bs => new BookedSeatDto
                    {
                        EventSeatId = bs.EventSeatId,
                        RowLabel = bs.EventSeat.Seat.RowLabel,
                        SeatNumber = bs.EventSeat.Seat.SeatNumber,
                        Price = bs.PriceAtBooking
                    }).ToList()
                })
                .ToListAsync();

            return bookings;
        }
    } }