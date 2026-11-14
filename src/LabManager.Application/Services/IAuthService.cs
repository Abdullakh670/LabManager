using LabManager.Domain.Entities;

namespace LabManager.Application.Services;

public interface IAuthService
{
    Task<User?> LoginAsync(string email, string password);
}