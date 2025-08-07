using LeaveTracking.Domain.Entities;
using LeaveTracking.Domain.Enums;
using LeaveTracking.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveTracking.Infrastructure.Seed
{
    public static class SeedData
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (await context.Users.AnyAsync()) return;

            // manager sifre Aa123456.
            var manager = new User
            {
                FirstName = "Muhammet Seyda",
                LastName = "Armağan",
                Email = "muhammedseyda@hotmail.com",
                PasswordHash = "$2a$11$kq6ktqevXuCiniqCmIGfj.sHdTkEsyl69ZGIy1Lzx4aAvwm2affYG",
                Role = UserRole.Manager,
                AnnualLeaveQuota = 14,
                CreatedAt = DateTime.Parse("2025-08-07 16:24:11.0490481"),
                CreatedBy = null,
                UpdatedAt = DateTime.MinValue,
                UpdatedBy = null
            };

            // Employee sifre: Aa123456.
            var employee = new User
            {
                FirstName = "Ahmet",
                LastName = "Said",
                Email = "yazilimcisarman@hotmail.com",
                PasswordHash = "$2a$11$hNa/vNtIseS3Df/h4QHH3uw3ckx.q54yJgNgPP0qe90rTSPN7dLvq",
                Role = UserRole.Employee,
                AnnualLeaveQuota = 3,
                CreatedAt = DateTime.Parse("2025-08-07 16:24:53.0622784"),
                CreatedBy = null,
                UpdatedAt = DateTime.MinValue,
                UpdatedBy = null
            };

            await context.Users.AddRangeAsync(manager, employee);
            await context.SaveChangesAsync();

            // Leave Requests
            var leave1 = new LeaveRequest
            {
                UserId = 2,
                StartDate = DateTime.Parse("2025-08-11"),
                EndDate = DateTime.Parse("2025-08-15"),
                LeaveType = LeaveType.Annual,
                Description = null,
                Status = LeaveStatus.Approved,
                RejectedReason = null,
                CreatedAt = DateTime.Parse("2025-08-07 15:10:36.2081395"),
                CreatedBy = "2",
                UpdatedAt = DateTime.Parse("2025-08-07 17:11:17.9616678"),
                UpdatedBy = "1"
            };

            var leave2 = new LeaveRequest
            {
                UserId = 2,
                StartDate = DateTime.Parse("2025-08-21"),
                EndDate = DateTime.Parse("2025-08-22"),
                LeaveType = LeaveType.Sick,
                Description = null,
                Status = LeaveStatus.Rejected,
                RejectedReason = "Hasta degilsin",
                CreatedAt = DateTime.Parse("2025-08-07 15:21:50.6550843"),
                CreatedBy = "2",
                UpdatedAt = DateTime.Parse("2025-08-07 17:22:51.7903670"),
                UpdatedBy = "1"
            };

            var leave3 = new LeaveRequest
            {
                UserId = 2,
                StartDate = DateTime.Parse("2025-08-18"),
                EndDate = DateTime.Parse("2025-08-25"),
                LeaveType = LeaveType.Annual,
                Description = "Ek yıllık izin",
                Status = LeaveStatus.Approved,
                RejectedReason = null,
                CreatedAt = DateTime.Parse("2025-08-07 15:21:50.6550843"),
                CreatedBy = "2",
                UpdatedAt = DateTime.Parse("2025-08-07 17:21:50.6550843"),
                UpdatedBy = "1"
            };

            await context.LeaveRequests.AddRangeAsync(leave1, leave2,leave3);
            await context.SaveChangesAsync();

        }
    }
}
