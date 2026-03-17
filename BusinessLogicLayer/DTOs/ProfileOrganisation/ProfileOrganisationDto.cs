using BusinessLogicLayer.DTOs.Organisations;
using BusinessLogicLayer.DTOs.Profile;
using BusinessLogicLayer.DTOs.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.DTOs.ProfileOrganisation
{
    public class CreateProfileOrganisationDto
    {
        public string ProfileId { get; set; } = string.Empty;
        public string OrganisationId { get; set; } = string.Empty;
        public bool? InvitationAccepted { get; set; }
        public bool IsOrgAdmin { get; set; } = false;
    }
    public class GetProfileOrganisationDto
    {
        public GetUsersListDto? Profile { get; set; }
        public GetOrganisationDto? Organisation { get; set; }
        public bool? InvitationAccepted { get; set; }
        public bool IsOrgAdmin { get; set; } = false;
    }
}
