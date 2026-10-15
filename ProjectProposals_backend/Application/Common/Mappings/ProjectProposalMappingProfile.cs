using AutoMapper;
using Application.Dtos;
using Application.Dtos.Requests;
using Application.Dtos.Responses;
using Domain.Entities;
using Domain.Enums;

namespace Application.Common.Mappings
{
    public class ProjectProposalMappingProfile : Profile
    {
        public ProjectProposalMappingProfile()
        {
            CreateMap<ProjectProposal, ProjectResponse>()
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.EstimatedAmount))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.EstimatedDuration))
                .ForMember(dest => dest.Area, opt => opt.MapFrom(src =>
                    src.ProjectProposalArea ?? (src.Area > 0 ? new Area { Id = src.Area, Name = src.Area.ToString() } : null)))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src =>
                    src.ProjectProposalType ?? (src.Type > 0 ? new ProjectType { Id = src.Type, Name = src.Type.ToString() } : null)))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src =>
                    src.ProjectProposalApprovalStatus ?? new ApprovalStatus(
                        Enum.IsDefined(typeof(ApprovalStatusEnum), src.Status)
                            ? ((ApprovalStatusEnum)src.Status).ToString()
                            : src.Status.ToString()) { Id = src.Status }))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.ProjectProposalUser))
                .ForMember(dest => dest.Steps, opt => opt.MapFrom(src => src.Steps));

            CreateMap<ProjectProposal, ProjectShortResponse>()
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.EstimatedAmount))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.EstimatedDuration))
                .ForMember(dest => dest.Area, opt => opt.MapFrom(src => src.ProjectProposalArea != null ? src.ProjectProposalArea.Name : string.Empty))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.ProjectProposalType != null ? src.ProjectProposalType.Name : string.Empty))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.ProjectProposalApprovalStatus != null ? src.ProjectProposalApprovalStatus.Name : string.Empty));

            CreateMap<ProjectProposalCreateRequest, ProjectProposalDto>()
                .ForMember(dest => dest.EstimatedAmount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.EstimatedDuration, opt => opt.MapFrom(src => src.Duration))
                .ForMember(dest => dest.CreateBy, opt => opt.MapFrom(src => src.User));

            CreateMap<ProjectProposal, ProjectProposalDto>().ReverseMap();
        }
    }
}
