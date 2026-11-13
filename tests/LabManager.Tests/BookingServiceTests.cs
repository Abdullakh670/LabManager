using LabManager.Application.Interfaces;
using LabManager.Application.Services;
using LabManager.Domain.Entities;
using LabManager.Domain.Enums;
using Moq;

namespace LabManager.Tests;

public class BookingServiceTests
{
    private readonly Mock<IEquipmentRepository> _equipmentRepo = new();
    private readonly Mock<IBookingRepository> _bookingRepo = new();
    private readonly BookingService _service;

    private readonly DateTime _start = new(2026, 12, 1, 10, 0, 0);
    private readonly DateTime _end = new(2026, 12, 1, 12, 0, 0);

    public BookingServiceTests()
    {
        _service = new BookingService(_equipmentRepo.Object, _bookingRepo.Object);
    }

    private void SetupEquipment(ResourceStatus status = ResourceStatus.Available) =>
        _equipmentRepo.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Equipment { Id = 1, Name = "Router", Status = status });

    [Fact]
    public async Task Create_WhenFree_Succeeds_AndSavesBooking()
    {
        SetupEquipment();
        _bookingRepo.Setup(r => r.HasConflictAsync(1, _start, _end)).ReturnsAsync(false);

        var result = await _service.CreateBookingAsync(1, "Abdullakh", _start, _end);

        Assert.True(result.Success);
        _bookingRepo.Verify(r => r.AddAsync(It.Is<Booking>(b =>
            b.EquipmentId == 1 && b.StudentName == "Abdullakh" &&
            b.StartTime == _start && b.EndTime == _end)), Times.Once);
    }

    [Fact]
    public async Task Create_WhenTimeConflicts_Fails_AndDoesNotSave()
    {
        SetupEquipment();
        _bookingRepo.Setup(r => r.HasConflictAsync(1, _start, _end)).ReturnsAsync(true);

        var result = await _service.CreateBookingAsync(1, "Abdullakh", _start, _end);

        Assert.False(result.Success);
        _bookingRepo.Verify(r => r.AddAsync(It.IsAny<Booking>()), Times.Never);
    }

    [Fact]
    public async Task Create_WhenEndBeforeStart_Fails()
    {
        var result = await _service.CreateBookingAsync(1, "Abdullakh", _end, _start);

        Assert.False(result.Success);
        _bookingRepo.Verify(r => r.AddAsync(It.IsAny<Booking>()), Times.Never);
    }

    [Fact]
    public async Task Create_WhenEquipmentInMaintenance_Fails()
    {
        SetupEquipment(ResourceStatus.Maintenance);

        var result = await _service.CreateBookingAsync(1, "Abdullakh", _start, _end);

        Assert.False(result.Success);
        _bookingRepo.Verify(r => r.AddAsync(It.IsAny<Booking>()), Times.Never);
    }

    [Fact]
    public async Task Create_WhenEquipmentNotFound_Fails()
    {
        _equipmentRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Equipment?)null);

        var result = await _service.CreateBookingAsync(99, "Abdullakh", _start, _end);

        Assert.False(result.Success);
    }

    [Fact]
    public async Task Create_WhenStudentNameEmpty_Fails()
    {
        SetupEquipment();

        var result = await _service.CreateBookingAsync(1, "  ", _start, _end);

        Assert.False(result.Success);
    }
}