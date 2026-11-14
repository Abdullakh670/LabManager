using LabManager.Application.Security;
using LabManager.WebUI.Services;
using LabManager.Application.Interfaces;
using LabManager.Application.Services;
using LabManager.Domain.Entities;
using LabManager.Domain.Enums;
using LabManager.Infrastructure.Data;
using LabManager.Infrastructure.Repositories;
using LabManager.WebUI.Components;
using LabManager.WebUI.Hubs;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSignalR();

builder.Services.AddScoped<IEquipmentRepository, EquipmentRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IEquipmentService, EquipmentService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<CurrentUserState>();

var app = builder.Build();

// Тестовые данные
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();

    if (!db.Equipments.Any())
    {
        db.Equipments.AddRange(
            new Equipment { Name = "Cisco Router 2911", Description = "Сетевой маршрутизатор для лабораторных", Status = ResourceStatus.Available },
            new Equipment { Name = "Cisco Switch 2960", Description = "Коммутатор на 24 порта", Status = ResourceStatus.Available },
            new Equipment { Name = "Oscilloscope", Description = "Цифровой осциллограф", Status = ResourceStatus.Maintenance }
        );
        db.SaveChanges();
        
        if (!db.Users.Any())
{
    db.Users.AddRange(
        new User { FullName = "Admin", Email = "admin@labmanager.local", PasswordHash = PasswordHasher.Hash("Admin123"), Role = UserRole.Admin },
        new User { FullName = "Abdullakh", Email = "student@labmanager.local", PasswordHash = PasswordHasher.Hash("Student123"), Role = UserRole.Student }
    );
    db.SaveChanges();
}
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapHub<BookingHub>("/bookinghub");

app.Run();