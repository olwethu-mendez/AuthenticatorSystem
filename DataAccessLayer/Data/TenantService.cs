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
                return _httpContextAccessor
                .HttpContext?
                .User?
                .Claims?
                .FirstOrDefault(c => c.Type == "ActiveOrgId")
                ?.Value;
            }
        }
    }
}
