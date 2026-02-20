using BusinessLogicLayer.DTOs.Authentication;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Data;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class TokenService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IServiceProvider _serviceProvider;
        public TokenService(IConfiguration config, UserManager<ApplicationUser> userManager, ApplicationDbContext context, IServiceProvider serviceProvider)
        {
            _config = config;
            _userManager = userManager;
            _context = context;
            _serviceProvider = serviceProvider;
        }
        public async Task<AuthResultDto> GenerateJwtToken(ApplicationUser user, bool stayLoggedIn, RefreshToken? existingRefreshToken)
        {
            var authClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.GivenName, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

            };

            var userRoles = await _userManager.GetRolesAsync(user!);
            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            bool hasProfile;
            var userProfile = await _context.Profiles.Where(x => x.UserId == user.Id).Include(x => x.User).FirstOrDefaultAsync();
            if (userProfile != null)
            {
                hasProfile = true;
                var phoneNumber = userProfile?.User?.CountryCode + userProfile?.User?.PhoneNumber;
                authClaims.AddRange(new List<Claim>()
                {
                    new Claim("FirstName", userProfile?.FirstName ?? string.Empty),
                    new Claim("LastName", userProfile?.LastName ?? string.Empty),
                    new Claim("PhoneNumber", phoneNumber ?? string.Empty),
                });
            }
            else hasProfile = false;
            authClaims.Add(new Claim("HasProfile", hasProfile.ToString().ToLower()));
            // The individual states
            bool emailVerified = !string.IsNullOrEmpty(user.Email) && user.EmailConfirmed;
            bool phoneVerified = !string.IsNullOrEmpty(user.PhoneNumber) && user.PhoneNumberConfirmed;
            bool isDeactivated = user.IsDeactivated;
            bool isDeactivatedByAdmin = user.IsDeactivatedByAdmin;

            // The aggregate "Is the user ready to use the app?"
            bool isFullyActive = !isDeactivatedByAdmin && !isDeactivated && (emailVerified || phoneVerified);

            // 1. The "Master Switch" claim
            authClaims.Add(new Claim("IsActivated", isFullyActive.ToString().ToLower()));

            // 2. The "Reason" claims (Frontend uses these for UI logic)
            authClaims.Add(new Claim("EmailConfirmed", emailVerified.ToString().ToLower()));
            authClaims.Add(new Claim("PhoneConfirmed", phoneVerified.ToString().ToLower()));
            authClaims.Add(new Claim("IsDeactivated", isDeactivated.ToString().ToLower()));
            authClaims.Add(new Claim("IsDeactivatedByAdmin", isDeactivatedByAdmin.ToString().ToLower()));

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Secret"]!));
            var token = new JwtSecurityToken(
                issuer: _config["JWT:Issuer"],
                audience: _config["JWT:Audience"],
                expires: stayLoggedIn == false ? DateTime.UtcNow.AddMinutes(15) : DateTime.UtcNow.AddMinutes(60),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );
            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
            var refreshToken = new RefreshToken();
            if (existingRefreshToken == null)
            {
                refreshToken = new RefreshToken()
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = user.Id,
                    JwtId = token.Id,
                    IsRevoked = false,
                    Token = Guid.NewGuid().ToString() + "." + Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)) + "." + Guid.NewGuid().ToString(),
                    StayLoggedIn = stayLoggedIn,
                    DateCreated = DateTime.UtcNow,
                    DateExpires = stayLoggedIn == false ? DateTime.UtcNow.AddHours(12) : DateTime.UtcNow.AddDays(30) //@Chat: or can i make this null or is that not allowed,
                };
                await _context.RefreshTokens.AddAsync(refreshToken);
                await _context.SaveChangesAsync();
                return new AuthResultDto
                {
                    Token = jwtToken,
                    RefreshToken = refreshToken.Token,
                    ExpirationDate = token.ValidTo
                };
            }
            else
            {
                existingRefreshToken.JwtId = token.Id;
                _context.RefreshTokens.Update(existingRefreshToken);
                await _context.SaveChangesAsync();
                return new AuthResultDto()
                {
                    Token = jwtToken,
                    RefreshToken = existingRefreshToken == null ? refreshToken.Token : existingRefreshToken.Token,
                    ExpirationDate = token.ValidTo,
                };
            }
        }

        public JwtSecurityToken DecodeToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            if (jwtToken == null)
                throw new SecurityTokenException("Invalid token");

            return jwtToken;
        }

        public async Task BlacklistToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var expiry = jwtToken.ValidTo; // Get the original expiry

            _context.TokenBlacklists.Add(new TokenBlacklist
            {
                Token = token,
                ExpirationDate = expiry
            });
            await _context.SaveChangesAsync();
        }
    }
}
