using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Data
{
    public class TenantService : ITenantService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TenantService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public string? TenantId
        {
            get
            {
                var context = _httpContextAccessor.HttpContext;
                var headerTenant = context?.Request.Headers["X-Tenant-Id"].FirstOrDefault();
                var userId = context?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(headerTenant) || string.IsNullOrEmpty(userId)) return null;

                using var scope = context?.RequestServices.CreateScope();
                var db = scope?.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var isMember = db?.ProfileOrganisations.Any(po =>
                    po.OrganisationId == headerTenant &&
                    po.Profile.UserId == userId &&
                    po.InvitationAccepted == true);

                return isMember == true ? headerTenant : null;
            }
        }
    }
}
