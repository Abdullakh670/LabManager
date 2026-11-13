using LabManager.Application.Services;
using LabManager.Domain.Entities;
using LabManager.Domain.Enums;
using LabManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LabManager.Infrastructure.Services;

public class EquipmentService : IEquipmentService
{
    private readonly ApplicationDbContext _context;

    // Внедряем базу данных через конструктор
    public EquipmentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Equipment>> GetAllEquipmentAsync()
    {
        return await _context.Equipments.ToListAsync();
    }

    public async Task<Equipment?> GetEquipmentByIdAsync(int id)
    {
        return await _context.Equipments.FindAsync(id);
    }

    public async Task<bool> BookEquipmentAsync(int equipmentId, string studentName, DateTime start, DateTime end)
    {
        if (end <= start)
            return false;

        var equipment = await _context.Equipments.FindAsync(equipmentId);

        // Если оборудования нет или оно не доступно, отказ
        if (equipment == null || equipment.Status != ResourceStatus.Available)
            return false;

        // Конфликт: диапазоны пересекаются, если newStart < existingEnd && newEnd > existingStart
        bool hasConflict = await _context.Bookings.AnyAsync(b =>
            b.EquipmentId == equipmentId &&
            start < b.EndTime &&
            end > b.StartTime);

        if (hasConflict)
            return false;

        var booking = new Booking
        {
            EquipmentId = equipmentId,
            StudentName = studentName,
            StartTime = start,
            EndTime = end
        };

        _context.Bookings.Add(booking);

        // Меняем статус, чтобы кнопка отключилась и статус стал «Занято»
        equipment.Status = ResourceStatus.Booked;

        await _context.SaveChangesAsync();
        return true;
    }
}