using DataAccessLayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class ProfileProject : ITenantEntity
    {
        public string ProfileId { get; set; } = string.Empty;
        public string ProjectId { get; set; } = string.Empty;
        public string OrganisationId { get; set ; } = string.Empty;
        public string? ProjectRole { get; set; }

        public virtual Profile? Profile { get; set; }
        public virtual OrganisationProject? Project { get; set; }
    }
}
