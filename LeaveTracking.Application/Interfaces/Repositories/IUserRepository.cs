using LeaveTracking.Domain.Entities;

namespace LeaveTracking.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserByEmailAsync(string email);

        Task<User?> GetUserByIdAsync(int id);

        Task CreateUserAsync(User user);

        Task UpdateAnnualLeaveQuotaAsync(int userId, int leaveDate);

        Task<int> GetUserAnnualLeaveQuota(int userId);

        Task<bool> CheckUserAsync(int userId);

        Task<List<User>> GetAllAsync();

        Task DeleteAsync(User user);

        Task<List<User>> GetAnnualLeaveQuotaLessThan(int quota);

        Task<User> GetManager();
    }
}