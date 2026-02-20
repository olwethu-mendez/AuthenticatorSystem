using DataAccessLayer.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class Profile
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string? GenderSelfDescription { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; } = string.Empty;

        //Maybe but not necessary
        public string? ProfilePictureUrl { get; set; }
        public string? ProfilePictureName { get; set; }

        //Even less likely unless specifically needed
        public string? Bio { get; set; }
        public DateTime? DateOfBirth { get; set; }

        public virtual ApplicationUser? User { get; set; }
    }
}
