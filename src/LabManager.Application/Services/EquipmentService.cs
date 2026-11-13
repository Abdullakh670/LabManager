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

    public async Task<bool> BookEquipmentAsync(int equipmentId, string studentName, DateTime start, DateTime end)
    {
        var result = await _bookingService.CreateBookingAsync(equipmentId, studentName, start, end);
        return result.Success;
    }
}