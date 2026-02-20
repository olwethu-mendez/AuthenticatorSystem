using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.DTOs.Profile
{
    public class GetProfileDto
    {
        public string? UserId { get; set; }
        public string? ProfileId { get; set; }
        public string? Username { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? EmailAddress { get; set; }
        public bool? EmailConfirmed { get; set; }
        public string? CountryCode { get; set; }
        public string? PhoneNumber { get; set; }
        public bool? PhoneNumberConfirmed { get; set; }
        public string? Gender { get; set; }
        public string? GenderSelfDescription { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? ProfilePictureName { get; set;}
        public int? PasswordLastUpdated { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
