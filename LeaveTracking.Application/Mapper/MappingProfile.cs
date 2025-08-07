using AutoMapper;
using LeaveTracking.Application.DTOs.EmailDTOs;
using LeaveTracking.Application.DTOs.LeaveRequestDTOs;
using LeaveTracking.Application.DTOs.UserDTOs;
using LeaveTracking.Domain.Entities;

namespace LeaveTracking.Application.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<LeaveRequest, CreateLeaveRequestDto>().ReverseMap();
            CreateMap<LeaveRequest, UpdateLeaveRequestDto>().ReverseMap();
            CreateMap<LeaveRequest, LeaveRequestDto>().ReverseMap();
            CreateMap<LeaveRequest, ResultLeaveRequestDto>().ReverseMap();
            CreateMap<LeaveRequest, EmailLeaveRequestDto>().ReverseMap();

            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<User, RegisterDto>().ReverseMap();
            CreateMap<User, ResultUserDto>().ReverseMap();
            CreateMap<User, EmailUserDto>().ReverseMap();
            CreateMap<User, UserLeaveSummaryDto>().ReverseMap();
        }
    }
}