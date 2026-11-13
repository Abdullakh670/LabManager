namespace LabManager.Application.Models;

public record BookingResult(bool Success, string? Error = null)
{
    public static BookingResult Ok() => new(true);
    public static BookingResult Fail(string error) => new(false, error);
}
