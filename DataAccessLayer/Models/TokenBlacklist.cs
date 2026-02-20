using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Models
{
    public class TokenBlacklist
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Token { get; set; } = string.Empty;
        public DateTime ExpirationDate { get; set; }
    }
}
