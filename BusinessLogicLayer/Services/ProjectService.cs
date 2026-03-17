using BusinessLogicLayer.DTOs.OrganisationProjects;
using BusinessLogicLayer.Infrastructure;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Data;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class ProjectService : IProjectServices
    {
        private readonly ITenantService _tenantService;
        private readonly ApplicationDbContext _context;
        public ProjectService(ITenantService tenantService, ApplicationDbContext context)
        {
            _tenantService = tenantService;
            _context = context;
        }
        public async Task CreateProject(CreateProjectDto payload)
        {
            var tenantId = _tenantService.TenantId;
            if (string.IsNullOrEmpty(tenantId)) throw new ClientError(400, "No tenant context found.");

            var project = new OrganisationProject
            {
                Name = payload.Name,
                Description = payload.Description,
                OrganisationId = tenantId,
            };
            _context.OrganisationProjects.Add(project);
            await _context.SaveChangesAsync();
        }

        public async Task<List<GetProjectsDto>> GetProjects()
        {
            return await _context.OrganisationProjects.Select(x => new GetProjectsDto
            {
                ProjectId = x.Id,
                Name = x.Name,
                OrganisationId = x.OrganisationId,
                ProjectImageUrl = x.ProjectImageUrl
            }).ToListAsync();
        }

        public async Task<GetProjectDto> GetProject(string projectId)
        {
            var project = await _context.OrganisationProjects
                .Select(x => new GetProjectDto
                {
                    ProjectId = projectId,
                    ProjectImageUrl= x.ProjectImageUrl,
                    OrganisationId= x.OrganisationId,
                    Name= x.Name,
                    Description= x.Description,
                    ProjectImageName = x.ProjectImageName
                })
                .FirstOrDefaultAsync(x => x.ProjectId == projectId);

            if (project == null) throw new ClientError(404, "Project not found in this organisation.");
            return project;
        }
    }
}
