using LabManager.Domain.Entities;
using LabManager.Domain.Enums;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace LabManager.WebUI.Services;

public record SessionUser(int Id, string FullName, UserRole Role);

public class CurrentUserState
{
    private const string Key = "currentUser";
    private readonly ProtectedSessionStorage _storage;
    private bool _initialized;

    public CurrentUserState(ProtectedSessionStorage storage) => _storage = storage;

    public SessionUser? Current { get; private set; }
    public bool IsLoggedIn => Current is not null;
    public bool IsAdmin => Current?.Role == UserRole.Admin;

    public async Task InitializeAsync()
    {
        if (_initialized) return;
        var result = await _storage.GetAsync<SessionUser>(Key);
        Current = result.Success ? result.Value : null;
        _initialized = true;
    }

    public async Task LoginAsync(User user)
    {
        Current = new SessionUser(user.Id, user.FullName, user.Role);
        await _storage.SetAsync(Key, Current);
        _initialized = true;
    }

    public async Task LogoutAsync()
    {
        Current = null;
        await _storage.DeleteAsync(Key);
    }
}