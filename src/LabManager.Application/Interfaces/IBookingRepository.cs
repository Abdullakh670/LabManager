using LabManager.Domain.Entities;

namespace LabManager.Application.Interfaces;

public interface IBookingRepository
{
    Task<bool> HasConflictAsync(int equipmentId, DateTime start, DateTime end);
    Task<List<int>> GetBusyEquipmentIdsAsync(DateTime moment);
    Task AddAsync(Booking booking);
    Task<List<Booking>> GetByUserAsync(int userId);
    Task<Booking?> GetByIdAsync(int id);
    Task DeleteAsync(Booking booking);
}