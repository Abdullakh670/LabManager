using LabManager.Application.Models;

namespace LabManager.Application.Services;

public interface IBookingService
{
    Task<BookingResult> CreateBookingAsync(int equipmentId, string studentName, DateTime start, DateTime end);
}
