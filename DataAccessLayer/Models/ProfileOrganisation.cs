using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class ProfileOrganisation
    {
        [ForeignKey("Profile")]
        public string ProfileId { get; set; } = string.Empty;
        [ForeignKey("Organisation")]
        public string OrganisationId { get; set; } = string.Empty;
        public bool? InvitationAccepted { get; set; }
        public bool? IsOrgAdmin { get; set; } = false;

        public virtual Organisation? Organisation { get; set; }
        public virtual Profile? Profile { get; set; }
    }
}
