using LabManager.Application.Interfaces;
using LabManager.Domain.Entities;
using LabManager.Domain.Enums;

namespace LabManager.Application.Services;

public class EquipmentService : IEquipmentService
{
    private readonly IEquipmentRepository _equipment;
    private readonly IBookingRepository _bookings;
    private readonly IBookingService _bookingService;

    public EquipmentService(IEquipmentRepository equipment, IBookingRepository bookings, IBookingService bookingService)
    {
        _equipment = equipment;
        _bookings = bookings;
        _bookingService = bookingService;
    }

    public async Task<List<Equipment>> GetAllEquipmentAsync()
    {
        var items = await _equipment.GetAllAsync();
        var busyIds = (await _bookings.GetBusyEquipmentIdsAsync(DateTime.Now)).ToHashSet();

        // «Занято» считается по броням на текущий момент, в базе хранится только Maintenance
        foreach (var e in items)
        {
            e.Status = e.Status == ResourceStatus.Maintenance
                ? ResourceStatus.Maintenance
                : busyIds.Contains(e.Id) ? ResourceStatus.Booked : ResourceStatus.Available;
        }

        return items;
    }

    public Task<Equipment?> GetEquipmentByIdAsync(int id) => _equipment.GetByIdAsync(id);

    public async Task<bool> BookEquipmentAsync(int equipmentId, int userId, DateTime start, DateTime end)
    {
        var result = await _bookingService.CreateBookingAsync(equipmentId, userId, start, end);
        return result.Success;
    }

    public async Task AddEquipmentAsync(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Название обязательно.");

        await _equipment.AddAsync(new Equipment
        {
            Name = name.Trim(),
            Description = description?.Trim() ?? string.Empty,
            Status = ResourceStatus.Available
        });
    }

    public async Task SetMaintenanceAsync(int id, bool maintenance)
    {
        var item = await _equipment.GetByIdAsync(id);
        if (item is null) return;

        item.Status = maintenance ? ResourceStatus.Maintenance : ResourceStatus.Available;
        await _equipment.UpdateAsync(item);
    }

    public Task DeleteEquipmentAsync(int id) => _equipment.DeleteAsync(id);
}