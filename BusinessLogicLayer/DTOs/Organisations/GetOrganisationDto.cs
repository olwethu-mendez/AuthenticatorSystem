using System.ComponentModel.DataAnnotations;

namespace BusinessLogicLayer.DTOs.Organisations
{
    public class GetOrganisationDto
    {
        public string OrganizationId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Subdomain { get; set; } = string.Empty;
        public string? OrganizationImageUrl { get; set; }
        public string? OrganizationHeaderImageUrl { get; set; }
    }
    public class GetMyOrganisationDto
    {
        public string OrganizationId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Subdomain { get; set; } = string.Empty;
        public string? OrganizationImageUrl { get; set; }
        public string? OrganizationHeaderImageUrl { get; set; }
        public bool? IsAdmin { get; set; }
    }
}