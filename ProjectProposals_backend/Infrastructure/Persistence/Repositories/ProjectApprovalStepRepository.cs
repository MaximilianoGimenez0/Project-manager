using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class ProjectApprovalStepRepository : IProjectApprovalStepRepository
    {
        private readonly ProjectApprovalDbContext _context;

        public ProjectApprovalStepRepository(ProjectApprovalDbContext context)
        {
            _context = context;
        }

        public async Task<long> Add(Domain.Entities.ProjectApprovalStep step)
        {
            var efStep = new Infrastructure.Persistence.Entities.ProjectApprovalStep
            {
                StepOrder = step.StepOrder,
                DecisionDate = step.DecisionDate,
                Observations = step.Observations,
                ProjectProposalId = step.ProjectProposalId,
                ApproverUserId = step.ApproverUserId,
                ApproverRoleId = step.ApproverRoleId,
                Status = step.Status,
            };

            await _context.ProjectApprovalSteps.AddAsync(efStep);
            await _context.SaveChangesAsync();

            return efStep.Id;
        }

        public async Task<bool> Delete(long id)
        {
            var efStep = await _context.ProjectApprovalSteps.FirstOrDefaultAsync(s => s.Id == id);
            if (efStep == null) return false;

            _context.ProjectApprovalSteps.Remove(efStep);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Domain.Entities.ProjectApprovalStep>> GetAll()
        {
            var efSteps = await _context.ProjectApprovalSteps
                .AsNoTracking()
                .Include(s => s.StepApproverRole)
                .Include(s => s.StepApprovalStatus)
                .Include(s => s.StepUser)
                .ToListAsync();

            var domainSteps = new List<Domain.Entities.ProjectApprovalStep>();

            foreach (var efStep in efSteps)
            {
                var step = new Domain.Entities.ProjectApprovalStep
                {
                    Id = efStep.Id,
                    StepOrder = efStep.StepOrder,
                    DecisionDate = efStep.DecisionDate,
                    Observations = efStep.Observations,
                    ProjectProposalId = efStep.ProjectProposalId,
                    ApproverUserId = efStep.ApproverUserId,
                    ApproverRoleId = efStep.ApproverRoleId,
                    Status = efStep.Status,
                    StepApproverRole = efStep.StepApproverRole != null ? new Domain.Entities.ApproverRole
                    {
                        Id = efStep.StepApproverRole.Id,
                        Name = efStep.StepApproverRole.Name
                    } : null,
                    StepApprovalStatus = efStep.StepApprovalStatus != null ? new Domain.Entities.ApprovalStatus(efStep.StepApprovalStatus.Name)
                    {
                        Id = efStep.StepApprovalStatus.Id
                    } : null,
                    StepUser = efStep.StepUser != null ? new Domain.Entities.User
                    {
                        Id = efStep.StepUser.Id,
                        Name = efStep.StepUser.Name,
                        Email = efStep.StepUser.Email,
                        Role = efStep.StepUser.Role
                    } : null
                };

                domainSteps.Add(step);
            }

            return domainSteps;
        }

        public async Task<Domain.Entities.ProjectApprovalStep?> GetById(long id)
        {
            var efStep = await _context.ProjectApprovalSteps
                .AsNoTracking()
                .Include(s => s.StepApproverRole)
                .Include(s => s.StepApprovalStatus)
                .Include(s => s.StepUser)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (efStep == null) return null;

            return new Domain.Entities.ProjectApprovalStep
            {
                Id = efStep.Id,
                StepOrder = efStep.StepOrder,
                DecisionDate = efStep.DecisionDate,
                Observations = efStep.Observations,
                ProjectProposalId = efStep.ProjectProposalId,
                ApproverUserId = efStep.ApproverUserId,
                ApproverRoleId = efStep.ApproverRoleId,
                Status = efStep.Status,
                StepApproverRole = efStep.StepApproverRole != null ? new Domain.Entities.ApproverRole
                {
                    Id = efStep.StepApproverRole.Id,
                    Name = efStep.StepApproverRole.Name
                } : null,
                StepApprovalStatus = efStep.StepApprovalStatus != null ? new Domain.Entities.ApprovalStatus(efStep.StepApprovalStatus.Name)
                {
                    Id = efStep.StepApprovalStatus.Id
                } : null,
                StepUser = efStep.StepUser != null ? new Domain.Entities.User
                {
                    Id = efStep.StepUser.Id,
                    Name = efStep.StepUser.Name,
                    Email = efStep.StepUser.Email,
                    Role = efStep.StepUser.Role
                } : null
            };
        }

        public async Task<bool> Update(Domain.Entities.ProjectApprovalStep step)
        {
            var efStep = await _context.ProjectApprovalSteps.FirstOrDefaultAsync(s => s.Id == step.Id);
            if (efStep == null) return false;

            efStep.StepOrder = step.StepOrder;
            efStep.DecisionDate = step.DecisionDate;
            efStep.Observations = step.Observations;
            efStep.ProjectProposalId = step.ProjectProposalId;
            efStep.ApproverUserId = step.ApproverUserId;
            efStep.ApproverRoleId = step.ApproverRoleId;
            efStep.Status = step.Status;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
