using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Models
{
    public class PreviousPasswords
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [ForeignKey("User")]
        public string UserId { get; set; } = string.Empty; // Assuming UserId is a string. Adjust the type based on your User model.
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime DateSet { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }
}
