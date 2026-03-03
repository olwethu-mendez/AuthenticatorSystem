using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class OrganisationInvitation
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string OrganisationId { get; set; } = string.Empty;
        public string EmailOrPhone { get; set; } = string.Empty;
        public string Token { get; set; } = Guid.NewGuid().ToString();
        public DateTime ExpiryDate { get; set; }
        public bool IsUsed { get; set; } = false;
    }
}
