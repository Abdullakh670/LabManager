using LabManager.Application.Interfaces;
using LabManager.Application.Security;
using LabManager.Domain.Entities;

namespace LabManager.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;

    public AuthService(IUserRepository users) => _users = users;

    public async Task<User?> LoginAsync(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrEmpty(password))
            return null;

        var user = await _users.GetByEmailAsync(email.Trim().ToLowerInvariant());
        if (user is null) return null;

        return PasswordHasher.Verify(password, user.PasswordHash) ? user : null;
    }
}