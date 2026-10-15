using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.Common.Services;
using Application.Features.ProjectProposals.Commands.CreateProjectProposal;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg => cfg.AddMaps(typeof(ProjectProposalMappingProfile).Assembly));
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateProjectProposalCommand).Assembly));
            services.AddScoped<IApprovalStepEngine, ApprovalStepEngine>();

            return services;
        }
    }
}
