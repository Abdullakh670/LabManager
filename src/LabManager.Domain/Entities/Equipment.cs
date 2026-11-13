using LabManager.Domain.Enums;

namespace LabManager.Domain.Entities;

public class Equipment
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ResourceStatus Status { get; set; } = ResourceStatus.Available;
}