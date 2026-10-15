using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class ProjectProposalRepository : IProjectProposalRepository
    {
        private readonly ProjectApprovalDbContext _context;

        public ProjectProposalRepository(ProjectApprovalDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Add(Domain.Entities.ProjectProposal project)
        {
            var steps = new List<Entities.ProjectApprovalStep>();

            if (project.Steps != null)
            {
                foreach (var step in project.Steps)
                {
                    var temp = new Entities.ProjectApprovalStep
                    {
                        Status = step.Status,
                        StepOrder = step.StepOrder,
                        ProjectProposalId = step.ProjectProposalId,
                        ApproverRoleId = step.ApproverRoleId
                    };

                    steps.Add(temp);
                }
            }

            var efProject = new Entities.ProjectProposal
            {
                Id = project.Id,
                Title = project.Title,
                Description = project.Description,
                EstimatedAmount = project.EstimatedAmount,
                EstimatedDuration = project.EstimatedDuration,
                CreateAt = project.CreateAt,
                Area = project.Area,
                Type = project.Type,
                Status = project.Status,
                CreateBy = project.CreateBy,
                ProjectApprovalSteps = steps
            };

            await _context.ProjectProposals.AddAsync(efProject);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(Guid id)
        {
            var efProject = await _context.ProjectProposals.FirstOrDefaultAsync(p => p.Id == id);
            if (efProject == null) return false;

            _context.ProjectProposals.Remove(efProject);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Domain.Entities.ProjectProposal>> GetAll()
        {
            var efProjects = await _context.ProjectProposals
                .AsNoTracking()
                .ToListAsync();

            var domainProjects = new List<Domain.Entities.ProjectProposal>();

            foreach (var efProject in efProjects)
            {
                var project = new Domain.Entities.ProjectProposal
                {
                    Id = efProject.Id,
                    Title = efProject.Title,
                    Description = efProject.Description,
                    EstimatedAmount = efProject.EstimatedAmount,
                    EstimatedDuration = efProject.EstimatedDuration,
                    CreateAt = efProject.CreateAt,
                    Area = efProject.Area,
                    Type = efProject.Type,
                    Status = efProject.Status,
                    CreateBy = efProject.CreateBy
                };
                domainProjects.Add(project);
            }

            return domainProjects;
        }

        public async Task<Domain.Entities.ProjectProposal?> GetById(Guid id)
        {
            var efProject = await _context.ProjectProposals
                .AsNoTracking()
                .Include(p => p.ProjectProposalArea)
                .Include(p => p.ProjectProposalType)
                .Include(p => p.ProjectProposalApprovalStatus)
                .Include(p => p.ProjectProposalUser)
                .Include(p => p.ProjectApprovalSteps)
                    .ThenInclude(s => s.StepApproverRole)
                .Include(p => p.ProjectApprovalSteps)
                    .ThenInclude(s => s.StepApprovalStatus)
                .Include(p => p.ProjectApprovalSteps)
                    .ThenInclude(s => s.StepUser)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (efProject == null) return null;

            var steps = new List<Domain.Entities.ProjectApprovalStep>();

            if (efProject.ProjectApprovalSteps != null)
            {
                foreach (var step in efProject.ProjectApprovalSteps.OrderBy(s => s.StepOrder))
                {
                    var temp = new Domain.Entities.ProjectApprovalStep
                    {
                        Id = step.Id,
                        StepOrder = step.StepOrder,
                        DecisionDate = step.DecisionDate,
                        Observations = step.Observations,
                        ProjectProposalId = step.ProjectProposalId,
                        ApproverUserId = step.ApproverUserId,
                        ApproverRoleId = step.ApproverRoleId,
                        Status = step.Status,
                        StepApproverRole = step.StepApproverRole != null ? new Domain.Entities.ApproverRole
                        {
                            Id = step.StepApproverRole.Id,
                            Name = step.StepApproverRole.Name
                        } : null,
                        StepApprovalStatus = step.StepApprovalStatus != null ? new Domain.Entities.ApprovalStatus(step.StepApprovalStatus.Name)
                        {
                            Id = step.StepApprovalStatus.Id
                        } : null,
                        StepUser = step.StepUser != null ? new Domain.Entities.User
                        {
                            Id = step.StepUser.Id,
                            Name = step.StepUser.Name,
                            Email = step.StepUser.Email,
                            Role = step.StepUser.Role
                        } : null
                    };

                    steps.Add(temp);
                }
            }

            var result = new Domain.Entities.ProjectProposal
            {
                Id = efProject.Id,
                Title = efProject.Title,
                Description = efProject.Description,
                EstimatedAmount = efProject.EstimatedAmount,
                EstimatedDuration = efProject.EstimatedDuration,
                CreateAt = efProject.CreateAt,
                Area = efProject.Area,
                Type = efProject.Type,
                Status = efProject.Status,
                CreateBy = efProject.CreateBy,
                ProjectProposalArea = efProject.ProjectProposalArea != null ? new Domain.Entities.Area
                {
                    Id = efProject.ProjectProposalArea.Id,
                    Name = efProject.ProjectProposalArea.Name
                } : null,
                ProjectProposalType = efProject.ProjectProposalType != null ? new Domain.Entities.ProjectType
                {
                    Id = efProject.ProjectProposalType.Id,
                    Name = efProject.ProjectProposalType.Name
                } : null,
                ProjectProposalApprovalStatus = efProject.ProjectProposalApprovalStatus != null ? new Domain.Entities.ApprovalStatus(efProject.ProjectProposalApprovalStatus.Name)
                {
                    Id = efProject.ProjectProposalApprovalStatus.Id
                } : null,
                ProjectProposalUser = efProject.ProjectProposalUser != null ? new Domain.Entities.User
                {
                    Id = efProject.ProjectProposalUser.Id,
                    Name = efProject.ProjectProposalUser.Name,
                    Email = efProject.ProjectProposalUser.Email,
                    Role = efProject.ProjectProposalUser.Role
                } : null,
                Steps = steps
            };

            return result;
        }

        public async Task<bool> Update(Domain.Entities.ProjectProposal project)
        {
            var efProject = await _context.ProjectProposals.FirstOrDefaultAsync(p => p.Id == project.Id);
            if (efProject == null) return false;

            efProject.Title = project.Title;
            efProject.Description = project.Description;
            efProject.EstimatedAmount = project.EstimatedAmount;
            efProject.EstimatedDuration = project.EstimatedDuration;
            efProject.CreateAt = project.CreateAt;
            efProject.Area = project.Area;
            efProject.Type = project.Type;
            efProject.Status = project.Status;
            efProject.CreateBy = project.CreateBy;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
