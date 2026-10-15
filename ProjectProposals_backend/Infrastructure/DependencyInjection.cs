using Domain.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("LocalProjectConnection");
            services.AddDbContext<ProjectApprovalDbContext>(options =>
                options.UseNpgsql(connectionString));

            services.AddScoped<IApprovalRuleRepository, ApprovalRuleRepository>();
            services.AddScoped<IApprovalStatusRepository, ApprovalStatusRepository>();
            services.AddScoped<IApproverRoleRepository, ApproverRoleRepository>();
            services.AddScoped<IAreaRepository, AreaRepository>();
            services.AddScoped<IProjectApprovalStepRepository, ProjectApprovalStepRepository>();
            services.AddScoped<IProjectProposalRepository, ProjectProposalRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IProjectTypeRepository, ProjectTypeRepository>();

            services.AddSingleton<Application.Common.Interfaces.IPasswordHasher, Infrastructure.Services.PasswordHasher>();
            services.AddScoped<Application.Common.Interfaces.IJwtTokenService, Infrastructure.Services.JwtTokenService>();

            return services;
        }
    }
}
