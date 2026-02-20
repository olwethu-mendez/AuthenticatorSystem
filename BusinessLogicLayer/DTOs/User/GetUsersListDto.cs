using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.DTOs.User
{
    public class GetUsersListDto
    {
        public string? UserId { get; set; }
        public string? ProfileId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Username { get; set; }
        public bool? EmailConfirmed { get; set; }
        public bool? PhoneNumberConfirmed { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public bool? IsDeactivated { get; set; }
        public bool? IsDeactivatedByAdmin { get; set; } = false;
    }
}
