using BusinessLogicLayer.DTOs.OrganisationProjects;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticatorSystem.Controllers
{
    [Route("api/projects")]
    [Authorize]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectServices _projectService;

        public ProjectController(IProjectServices projectService)
        {
            _projectService = projectService;
        }
        [HttpPost("create")]
        [Authorize(Policy = UserPolicies.MustHaveProfile)]
        [Authorize(Policy = UserPolicies.MustBeActivated)]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto payload)
        {
            await _projectService.CreateProject(payload);
            return Created();
        }

        [HttpGet("all")]
        [Authorize(Policy = UserPolicies.MustHaveProfile)]
        public async Task<ActionResult<List<GetProjectsDto>>> GetProjects()
        {
            var results = await _projectService.GetProjects();
            return Ok(results);
        }

        [HttpGet("{projectId}")]
        [Authorize(Policy = UserPolicies.MustHaveProfile)]
        public async Task<ActionResult<GetProjectDto>> GetProjectById(string projectId)
        {
            var result = await _projectService.GetProject(projectId);
            return Ok(result);
        }
    }
}
