using BusinessLogicLayer.DTOs.OrganisationProjects;
using System.ComponentModel.DataAnnotations;

namespace BusinessLogicLayer.DTOs.Organisations
{
    public class GetOrganisationDto
    {
        public string OrganizationId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Subdomain { get; set; } = string.Empty;
        public bool? IsPublic { get; set; }
        public string? OrganizationImageUrl { get; set; }
        public string? OrganizationHeaderImageUrl { get; set; }
        public List<GetProjectsDto>? Projects { get; set; }
        public string? Status { get; set; }
    }
    public class GetMyOrganisationDto
    {
        public string OrganizationId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Subdomain { get; set; } = string.Empty;
        public string? OrganizationImageUrl { get; set; }
        public bool? IsPublic { get; set; }
        public bool? IsAdmin { get; set; }
        public string? Status { get; set; }
    }
    public class GetOrganisationsDto
    {
        public string OrganizationId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Subdomain { get; set; } = string.Empty;
        public string? OrganizationImageUrl { get; set; }
        public bool? IsPublic { get; set; }
        public string? Status { get; set; }
    }
}