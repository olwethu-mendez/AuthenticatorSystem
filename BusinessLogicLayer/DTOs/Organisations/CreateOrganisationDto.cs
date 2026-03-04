using BusinessLogicLayer.Helper;
using BusinessLogicLayer.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace BusinessLogicLayer.DTOs.Organisations
{
    public class CreateOrganisationDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [ValidSubdomain]
        public string Subdomain { get; set; } = MultiTenantHelper.ToSubdomain(nameof(Name));
        public bool? IsPublic { get; set; }
        public IFormFile? OrganizationImage { get; set; }
        public IFormFile? OrganizationHeaderImage { get; set; }
    }
}