using LabManager.Application.Interfaces;
using LabManager.Domain.Entities;
using LabManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LabManager.Infrastructure.Repositories;

public class EquipmentRepository : IEquipmentRepository
{
    private readonly ApplicationDbContext _context;

    public EquipmentRepository(ApplicationDbContext context) => _context = context;

    // AsNoTracking: вычисленный статус не должен случайно сохраниться в базу
    public Task<List<Equipment>> GetAllAsync() =>
        _context.Equipments.AsNoTracking().OrderBy(e => e.Id).ToListAsync();

    public async Task<Equipment?> GetByIdAsync(int id) =>
        await _context.Equipments.FindAsync(id);

    public async Task AddAsync(Equipment equipment)
    {
        _context.Equipments.Add(equipment);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Equipment equipment)
    {
        _context.Equipments.Update(equipment);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Equipments.FindAsync(id);
        if (entity is null) return;

        _context.Equipments.Remove(entity);
        await _context.SaveChangesAsync();
    }
}