using LabManager.Application.Interfaces;
using LabManager.Domain.Entities;
using LabManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LabManager.Infrastructure.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly ApplicationDbContext _context;

    public BookingRepository(ApplicationDbContext context) => _context = context;

    // Пересечение: newStart < existingEnd && newEnd > existingStart
    public Task<bool> HasConflictAsync(int equipmentId, DateTime start, DateTime end) =>
        _context.Bookings.AnyAsync(b =>
            b.EquipmentId == equipmentId &&
            start < b.EndTime &&
            end > b.StartTime);

    public Task<List<int>> GetBusyEquipmentIdsAsync(DateTime moment) =>
        _context.Bookings
            .Where(b => b.StartTime <= moment && b.EndTime > moment)
            .Select(b => b.EquipmentId)
            .Distinct()
            .ToListAsync();

    public async Task AddAsync(Booking booking)
    {
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();
    }

    public Task<List<Booking>> GetByUserAsync(int userId) =>
        _context.Bookings
            .Include(b => b.Equipment)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.StartTime)
            .ToListAsync();

    public Task<Booking?> GetByIdAsync(int id) =>
        _context.Bookings.FirstOrDefaultAsync(b => b.Id == id);

    public async Task DeleteAsync(Booking booking)
    {
        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync();
    }
}