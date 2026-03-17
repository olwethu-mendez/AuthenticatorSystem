using DataAccessLayer.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Models
{
    public class Organisation
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Subdomain { get; set; } = string.Empty;
        public string? OrganizationImageUrl { get; set; }
        public string? OrganizationImageName { get; set; }
        public string? OrganizationHeaderImageUrl { get; set; }
        public string? OrganizationHeaderImageName { get; set; }
        public bool IsPublic { get; set; } = false;
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
        public string? Status { get; set; } = StatusValue.Activated;

        public virtual ICollection<ProfileOrganisation>? ProfileOrganisations { get; set; }
        public virtual ICollection<OrganisationProject>? OrganisationProjects { get; set; }
    }
}