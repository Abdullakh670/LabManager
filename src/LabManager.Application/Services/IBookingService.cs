using LabManager.Application.Models;
using LabManager.Domain.Entities;

namespace LabManager.Application.Services;

public interface IBookingService
{
    Task<BookingResult> CreateBookingAsync(int equipmentId, int userId, DateTime start, DateTime end);
    Task<List<Booking>> GetUserBookingsAsync(int userId);
    Task<BookingResult> CancelBookingAsync(int bookingId, int userId);
}