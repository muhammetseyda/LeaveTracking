using LeaveTracking.Domain.Entities;

namespace LeaveTracking.Application.Interfaces.Repositories
{
    public interface ILeaveRequestRepository
    {
        Task<List<LeaveRequest>> GetAllAsync();

        Task<LeaveRequest?> GetByIdAsync(int id);

        Task<List<LeaveRequest>> GetByUserIdAsync(int userId);

        Task<List<LeaveRequest>> GetPendingAsync();

        Task CreateAsync(LeaveRequest leaveRequest);

        Task<LeaveRequest> UpdateAsync(LeaveRequest leaveRequest);

        Task<bool> HasConflictAsync(int userId, DateTime start, DateTime end);

        Task<bool> HasConflictByUserAsync(DateTime start, DateTime end);

        Task DeleteAsync(LeaveRequest leaveRequest);
    }
}