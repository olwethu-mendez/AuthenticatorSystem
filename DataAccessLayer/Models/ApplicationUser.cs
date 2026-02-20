using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? CountryCode { get; set; }
        [PersonalData]
        [MaxLength(9)]
        [MinLength(9)]
        public override string? PhoneNumber { get; set; }
        public bool IsDeactivated { get; set; } = false;
        public bool IsDeactivatedByAdmin { get; set; } = false;
        public DateTime? DeactivatedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
