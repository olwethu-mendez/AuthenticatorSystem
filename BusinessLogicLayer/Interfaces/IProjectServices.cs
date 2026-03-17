using BusinessLogicLayer.DTOs.OrganisationProjects;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
    public interface IProjectServices
    {
        Task CreateProject(CreateProjectDto payload);
        Task<GetProjectDto> GetProject(string projectId);
        Task<List<GetProjectsDto>> GetProjects();
    }
}
