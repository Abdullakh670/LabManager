using LabManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LabManager.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Equipment> Equipments { get; set; }
    public DbSet<Booking> Bookings { get; set; }
}