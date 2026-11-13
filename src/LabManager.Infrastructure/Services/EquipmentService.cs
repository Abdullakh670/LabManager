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
        var equipment = await _context.Equipments.FindAsync(equipmentId);
        
        // Если оборудования нет или оно сломано/уже занято — отказ
        if (equipment == null || equipment.Status != ResourceStatus.Available)
        {
            return false; 
        }

// Создаем новую запись о бронировании
        var booking = new Booking
        {
            EquipmentId = equipmentId,
            StudentName = studentName,
            StartTime = start,
            EndTime = end
        };
        // Сохраняем в базу
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        return true;
    }
}