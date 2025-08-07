using LeaveTracking.Application.Interfaces.Repositories;
using LeaveTracking.Domain.Entities;
using LeaveTracking.Domain.Enums;
using LeaveTracking.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace LeaveTracking.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CheckUserAsync(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user != null)
                return true;
            return false;
        }

        public async Task CreateUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetUserAnnualLeaveQuota(int userId)
        {
            var userQouta = await _context.Users.Where(x => x.Id == userId).Select(x => x.AnnualLeaveQuota).FirstOrDefaultAsync();
            return userQouta;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            var result = await _context.Users.Include(u => u.LeaveRequests).FirstOrDefaultAsync(u => u.Email == email);
            return result;
        }

        public async Task UpdateAnnualLeaveQuotaAsync(int userId, int leaveDate)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            user.AnnualLeaveQuota -= leaveDate;
            if (user.AnnualLeaveQuota < 0)
            {
                user.AnnualLeaveQuota = 0;
            }
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<List<User>> GetAllAsync()
        {
            var users = await _context.Users.Include(u => u.LeaveRequests).ToListAsync();
            return users;
        }

        public async Task DeleteAsync(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public Task<User?> GetUserByIdAsync(int id)
        {
            var result = _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            return result;
        }

        public async Task<List<User>> GetAnnualLeaveQuotaLessThan(int quota)
        {
            var result = await _context.Users.Where(u => u.AnnualLeaveQuota < quota).ToListAsync();
            return result;
        }

        public async Task<User> GetManager()
        {
            var result = await _context.Users.Where(u => u.Role == UserRole.Manager).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            return result;
        }
    }
}