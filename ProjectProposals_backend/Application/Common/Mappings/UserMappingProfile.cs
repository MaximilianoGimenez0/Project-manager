using System;
using AutoMapper;
using Application.Dtos;
using Application.Dtos.Responses;
using Domain.Entities;
using Domain.Enums;

namespace Application.Common.Mappings
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<User, UserResponse>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src =>
                    src.UserRole ?? (src.Role > 0
                        ? new ApproverRole
                        {
                            Id = src.Role,
                            Name = Enum.IsDefined(typeof(ApproverRoleEnum), src.Role)
                                ? ((ApproverRoleEnum)src.Role).ToString()
                                : src.Role.ToString()
                        }
                        : null)));

            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
        }
    }
}
