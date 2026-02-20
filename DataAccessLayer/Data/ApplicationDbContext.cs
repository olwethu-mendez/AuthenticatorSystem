using DataAccessLayer.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Data
{
    public class ApplicationDbContext:IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<PreviousPasswords> PreviousPasswords { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<TokenBlacklist> TokenBlacklists{ get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
