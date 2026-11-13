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

    public async Task<BookingResult> CreateBookingAsync(int equipmentId, string studentName, DateTime start, DateTime end)
    {
        if (string.IsNullOrWhiteSpace(studentName))
            return BookingResult.Fail("Не указано имя студента.");

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
            StudentName = studentName,
            StartTime = start,
            EndTime = end
        });

        return BookingResult.Ok();
    }
}
