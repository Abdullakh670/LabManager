   using LabManager.Application.Interfaces;
   using LabManager.Domain.Entities;
   using LabManager.Infrastructure.Data;
   using Microsoft.EntityFrameworkCore;

   namespace LabManager.Infrastructure.Repositories;

   public class UserRepository : IUserRepository
   {
       private readonly ApplicationDbContext _context;

       public UserRepository(ApplicationDbContext context) => _context = context;

       public Task<User?> GetByEmailAsync(string email) =>
           _context.Users.FirstOrDefaultAsync(u => u.Email == email);
   }