using LabManager.Domain.Entities;

namespace LabManager.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
}