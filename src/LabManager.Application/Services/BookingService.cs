using LabManager.Application.Interfaces;
using LabManager.Application.Models;
using LabManager.Domain.Entities;
using LabManager.Domain.Enums;

namespace LabManager.Application.Services;

public class BookingService : IBookingService
{
    private readonly IEquipmentRepository _equipment;
    private readonly IBookingRepository _bookings;

    public BookingService(IEquipmentRepository equipment, IBookingRepository bookings)
    {
        _equipment = equipment;
        _bookings = bookings;
    }

    public async Task<BookingResult> CreateBookingAsync(int equipmentId, int userId, DateTime start, DateTime end)
    {
        if (userId <= 0)
            return BookingResult.Fail("Не указан пользователь.");

        if (end <= start)
            return BookingResult.Fail("Время окончания должно быть позже начала.");

        var equipment = await _equipment.GetByIdAsync(equipmentId);
        if (equipment == null)
            return BookingResult.Fail("Оборудование не найдено.");

        if (equipment.Status == ResourceStatus.Maintenance)
            return BookingResult.Fail("Оборудование на обслуживании.");

        if (await _bookings.HasConflictAsync(equipmentId, start, end))
            return BookingResult.Fail("Это время уже занято другим бронированием.");

        await _bookings.AddAsync(new Booking
        {
            EquipmentId = equipmentId,
            UserId = userId,
            StartTime = start,
            EndTime = end
        });

        return BookingResult.Ok();
    }

    public Task<List<Booking>> GetUserBookingsAsync(int userId) => _bookings.GetByUserAsync(userId);

    public async Task<BookingResult> CancelBookingAsync(int bookingId, int userId)
    {
        var booking = await _bookings.GetByIdAsync(bookingId);
        if (booking == null)
            return BookingResult.Fail("Бронь не найдена.");

        if (booking.UserId != userId)
            return BookingResult.Fail("Можно отменять только свои брони.");

        await _bookings.DeleteAsync(booking);
        return BookingResult.Ok();
    }
}