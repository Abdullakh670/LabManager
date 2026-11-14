using LabManager.Application.Interfaces;
using LabManager.Application.Security;
using LabManager.Application.Services;
using LabManager.Domain.Entities;
using LabManager.Domain.Enums;
using Moq;

namespace LabManager.Tests;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _users = new();
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _service = new AuthService(_users.Object);
    }

    private void SetupUser(string email, string password, UserRole role = UserRole.Student) =>
        _users.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(new User
        {
            Id = 1,
            FullName = "Test User",
            Email = email,
            PasswordHash = PasswordHasher.Hash(password),
            Role = role
        });

    [Fact]
    public async Task Login_WithCorrectCredentials_ReturnsUser()
    {
        SetupUser("admin@labmanager.local", "Admin123", UserRole.Admin);

        var user = await _service.LoginAsync("admin@labmanager.local", "Admin123");

        Assert.NotNull(user);
        Assert.Equal(UserRole.Admin, user!.Role);
    }

    [Fact]
    public async Task Login_WithWrongPassword_ReturnsNull()
    {
        SetupUser("admin@labmanager.local", "Admin123");

        var user = await _service.LoginAsync("admin@labmanager.local", "WrongPassword");

        Assert.Null(user);
    }

    [Fact]
    public async Task Login_WithUnknownEmail_ReturnsNull()
    {
        _users.Setup(r => r.GetByEmailAsync("nobody@labmanager.local")).ReturnsAsync((User?)null);

        var user = await _service.LoginAsync("nobody@labmanager.local", "Admin123");

        Assert.Null(user);
    }

    [Theory]
    [InlineData("", "Admin123")]
    [InlineData("   ", "Admin123")]
    [InlineData("admin@labmanager.local", "")]
    public async Task Login_WithEmptyFields_ReturnsNull_AndDoesNotQueryDatabase(string email, string password)
    {
        var user = await _service.LoginAsync(email, password);

        Assert.Null(user);
        _users.Verify(r => r.GetByEmailAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Login_EmailIsCaseInsensitive()
    {
        SetupUser("student@labmanager.local", "Student123");

        var user = await _service.LoginAsync("  STUDENT@LabManager.Local ", "Student123");

        Assert.NotNull(user);
    }

    [Fact]
    public void Hash_DoesNotStorePlainPassword_AndDiffersEachTime()
    {
        var first = PasswordHasher.Hash("Admin123");
        var second = PasswordHasher.Hash("Admin123");

        Assert.DoesNotContain("Admin123", first);
        Assert.NotEqual(first, second);
    }

    [Fact]
    public void Verify_ReturnsTrueOnlyForCorrectPassword()
    {
        var hash = PasswordHasher.Hash("Admin123");

        Assert.True(PasswordHasher.Verify("Admin123", hash));
        Assert.False(PasswordHasher.Verify("admin123", hash));
    }
}