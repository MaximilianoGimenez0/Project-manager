using System;
using AutoMapper;
using Application.Dtos;
using Application.Dtos.Responses;
using Domain.Entities;
using Domain.Enums;

namespace Application.Common.Mappings
{
    public class ProjectApprovalStepMappingProfile : Profile
    {
        public ProjectApprovalStepMappingProfile()
        {
            CreateMap<ProjectApprovalStep, ApprovalStepResponse>()
                .ForMember(dest => dest.ApproverUser, opt => opt.MapFrom(src => src.StepUser))
                .ForMember(dest => dest.ApproverRole, opt => opt.MapFrom(src =>
                    src.StepApproverRole ?? new ApproverRole
                    {
                        Id = src.ApproverRoleId,
                        Name = Enum.IsDefined(typeof(ApproverRoleEnum), src.ApproverRoleId)
                            ? ((ApproverRoleEnum)src.ApproverRoleId).ToString()
                            : src.ApproverRoleId.ToString()
                    }))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src =>
                    src.StepApprovalStatus ?? new ApprovalStatus(
                        Enum.IsDefined(typeof(ApprovalStatusEnum), src.Status)
                            ? ((ApprovalStatusEnum)src.Status).ToString()
                            : src.Status.ToString()) { Id = src.Status }));

            CreateMap<ProjectApprovalStep, ProjectApprovalStepDto>().ReverseMap();
        }
    }
}
