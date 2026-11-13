using LabManager.Domain.Entities;

namespace LabManager.Application.Services;

public interface IEquipmentService
{
    // Получить всё оборудование
    Task<List<Equipment>> GetAllEquipmentAsync();
    
    // Получить конкретное оборудование по ID
    Task<Equipment?> GetEquipmentByIdAsync(int id);
    
    // Забронировать оборудование
    Task<bool> BookEquipmentAsync(int equipmentId, string studentName, DateTime start, DateTime end);
}