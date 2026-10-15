using AutoMapper;
using Application.Dtos;
using Application.Dtos.Responses;
using Domain.Entities;

namespace Application.Common.Mappings
{
    public class CommonMappingProfile : Profile
    {
        public CommonMappingProfile()
        {
            CreateMap<Area, GenericResponse>();
            CreateMap<ApprovalStatus, GenericResponse>();
            CreateMap<ProjectType, GenericResponse>();
            CreateMap<Domain.Entities.ApproverRole, GenericResponse>();

            CreateMap<AreaDto, GenericResponse>();
            CreateMap<ApprovalStatusDto, GenericResponse>();
            CreateMap<ProjectTypeDto, GenericResponse>();
            CreateMap<ApproverRoleDto, GenericResponse>();
        }
    }
}
