using DataAccessLayer.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class OrganisationProject : ITenantEntity
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [ForeignKey("Organisation")]
        public string OrganisationId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ProjectImageUrl { get; set; }
        public string? ProjectImageName { get; set; }

        public virtual Organisation? Organisation { get; set; }
        public virtual ICollection<Profile>? Profiles { get; set; }
    }
}
