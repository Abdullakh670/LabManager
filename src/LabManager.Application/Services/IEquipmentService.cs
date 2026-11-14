using LabManager.Domain.Entities;

namespace LabManager.Application.Services;

public interface IEquipmentService
{
    Task<List<Equipment>> GetAllEquipmentAsync();
    Task<Equipment?> GetEquipmentByIdAsync(int id);
    Task<bool> BookEquipmentAsync(int equipmentId, int userId, DateTime start, DateTime end);

    Task AddEquipmentAsync(string name, string description);
    Task SetMaintenanceAsync(int id, bool maintenance);
    Task DeleteEquipmentAsync(int id);
}