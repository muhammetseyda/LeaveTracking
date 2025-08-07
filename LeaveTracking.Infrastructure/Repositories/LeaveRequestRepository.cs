using LeaveTracking.Application.Interfaces.Repositories;
using LeaveTracking.Domain.Entities;
using LeaveTracking.Domain.Enums;
using LeaveTracking.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace LeaveTracking.Infrastructure.Repositories
{
    public class LeaveRequestRepository : ILeaveRequestRepository
    {
        private readonly AppDbContext _context;

        public LeaveRequestRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(LeaveRequest leaveRequest)
        {
            await _context.LeaveRequests.AddAsync(leaveRequest);
            await _context.SaveChangesAsync();
        }

        public async Task<List<LeaveRequest>> GetAllAsync()
        {
            var result = await _context.LeaveRequests.Include(x => x.User).OrderDescending().ToListAsync();
            return result;
        }

        public Task<LeaveRequest?> GetByIdAsync(int id)
        {
            var result = _context.LeaveRequests.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == id);
            return result;
        }

        public Task<List<LeaveRequest>> GetByUserIdAsync(int userId)
        {
            var result = _context.LeaveRequests.Where(x => x.UserId == userId).ToListAsync();
            return result;
        }

        public async Task<List<LeaveRequest>> GetPendingAsync()
        {
            var result = await _context.LeaveRequests.Include(x => x.User).Where(x => x.Status == 0).ToListAsync();
            return result;
        }

        public async Task<bool> HasConflictAsync(int userId, DateTime start, DateTime end)
        {
            return await _context.LeaveRequests.AnyAsync(x => x.UserId == userId && x.Status == LeaveStatus.Approved &&
            ((start >= x.StartDate && start <= x.EndDate) ||
            (end >= x.StartDate && end <= x.EndDate) ||
            (start <= x.StartDate && end >= x.EndDate)));
        }

        public async Task<bool> HasConflictByUserAsync(DateTime start, DateTime end)
        {
            var result = await _context.LeaveRequests.Where(x => x.Status == LeaveStatus.Approved && x.StartDate == start && x.EndDate == end).AnyAsync();
            return result;
        }

        public async Task<LeaveRequest> UpdateAsync(LeaveRequest leaveRequest)
        {
            var item = await _context.LeaveRequests.FirstOrDefaultAsync(x => x.Id == leaveRequest.Id);
            item.Status = leaveRequest.Status;
            item.RejectedReason = leaveRequest.RejectedReason;
            item.UpdatedAt = DateTime.UtcNow;
            _context.LeaveRequests.Update(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task DeleteAsync(LeaveRequest leaveRequest)
        {
            _context.LeaveRequests.Remove(leaveRequest);
            await _context.SaveChangesAsync();
        }
    }
}