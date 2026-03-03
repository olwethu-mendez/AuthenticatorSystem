using DataAccessLayer.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly ITenantService _tenantService;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Organisation> Organisations { get; set; }
        public DbSet<PreviousPasswords> PreviousPasswords { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<TokenBlacklist> TokenBlacklists { get; set; }
        public DbSet<ProfileOrganisation> ProfileOrganisations { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ProfileOrganisation>()
                .HasKey(po => new { po.ProfileId, po.OrganisationId });

            // Apply Global Filter to any entity implementing ITenantEntity
            // This automatically adds "WHERE OrganisationId = 'xxx'" to all queries
            builder.Entity<ProfileOrganisation>()
                .HasQueryFilter(po => po.OrganisationId == _tenantService.TenantId);

            // If you create other tables like 'Project' or 'Invoice', do the same:
            // builder.Entity<Project>().HasQueryFilter(p => p.OrganisationId == _tenantService.TenantId);
        }
    }
}
