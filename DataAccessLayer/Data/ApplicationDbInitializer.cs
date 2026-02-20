using BusinessLogicLayer.Infrastructure;
using DataAccessLayer.Infrastructure;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Data
{
    public class ApplicationDbInitializer
    {
        public static async Task SeedRoles(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                if(!await roleManager.RoleExistsAsync(UserRoles.Admin))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
                if(!await roleManager.RoleExistsAsync(UserRoles.General))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.General));
            }
        }
        public static async Task SeedAdmin(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var admin = new ApplicationUser()
                {
                    Email = "admin@authenticator-systems.com",
                    UserName = "admin@authenticator-systems.com",
                    CountryCode = "+27",
                    PhoneNumber = "812345678",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                };
                var adminUser = serviceScope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
                if (adminUser == null)
                {
                    throw new Exception("UserManager service is not available.");
                }
                var getAdmin = await adminUser.FindByEmailAsync(admin.Email);
                if (getAdmin == null)
                {
                    var results = await adminUser.CreateAsync(admin, "Admin@123");
                    if (results.Succeeded)
                    {
                        await adminUser.AddToRoleAsync(admin, UserRoles.Admin);
                        // Create profile for admin
                        var profile = new Profile
                        {
                            UserId = admin.Id,
                            FirstName = "System",
                            LastName = "Administrator",
                            Gender = UserGender.PreferNotToSay,
                        };
                        var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                        context.Profiles.Add(profile);
                        await context.SaveChangesAsync();
                    }
                    else {
                        throw new Exception("Failed to create admin user during seeding.");
                    }
                }
            }
        }
    }
}
