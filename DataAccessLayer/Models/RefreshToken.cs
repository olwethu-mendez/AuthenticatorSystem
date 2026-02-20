using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Models
{
    public class RefreshToken
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [ForeignKey("User")]
        public string UserId { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string JwtId { get; set; } = string.Empty;
        public bool IsRevoked { get; set; }
        public bool StayLoggedIn { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public DateTime? DateExpires { get; set; }

        public virtual ApplicationUser? User { get; set; }
    }
}
