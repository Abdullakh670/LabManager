using Microsoft.AspNetCore.SignalR;

namespace LabManager.WebUI.Hubs;

public class BookingHub : Hub
{
    // Клиент вызывает этот метод после брони, сервер рассылает событие ВСЕМ клиентам
    public async Task NotifyChanged()
    {
        await Clients.All.SendAsync("EquipmentUpdated");
    }
}