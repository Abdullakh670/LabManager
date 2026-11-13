using LabManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using LabManager.WebUI.Components;
using LabManager.Domain.Entities;
using LabManager.Domain.Enums;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Добавляем тестовые данные
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    if (!db.Equipments.Any())
    {
        db.Equipments.AddRange(
            new Equipment { Name = "Cisco Router 2911", Description = "Сетевой маршрутизатор для лабораторных", Status = ResourceStatus.Available },
            new Equipment { Name = "Cisco Switch 2960", Description = "Коммутатор на 24 порта", Status = ResourceStatus.Available },
            new Equipment { Name = "Oscilloscope", Description = "Цифровой осциллограф", Status = ResourceStatus.Maintenance }
        );
        db.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
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

app.Run();