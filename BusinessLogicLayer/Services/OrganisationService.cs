using Amazon;
using BusinessLogicLayer.DTOs.Authentication;
using BusinessLogicLayer.DTOs.Organisations;
using BusinessLogicLayer.Infrastructure;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Data;
using DataAccessLayer.Infrastructure;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class OrganisationService : IOrganisationService
    {
        private readonly ContextAccessorService _contextAccessorService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly R2Service _r2Service;
        private readonly ApplicationDbContext _context;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TokenService _tokenService;

        public OrganisationService(ContextAccessorService contextAccessorService, UserManager<ApplicationUser> userManager, R2Service r2Service, ApplicationDbContext context, IServiceScopeFactory serviceScope, TokenService tokenService)
        {
            _contextAccessorService = contextAccessorService;
            _userManager = userManager;
            _r2Service = r2Service;
            _context = context;
            _scopeFactory = serviceScope;
            _tokenService = tokenService;
        }

        public async Task AcceptInvitation(string organisationId, bool invitationAccepted)
        {
            var currentUserId = _contextAccessorService.GetCurrentUserId();
            if (currentUserId == null) throw new ClientError(401, "Failed to retrieve currently authenticated user");
            var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == currentUserId);
            if (profile == null) throw new ClientError(404, "Current User Profile not found");

            var profileOrganisation = await _context.ProfileOrganisations.FirstOrDefaultAsync(x => x.OrganisationId == organisationId && x.ProfileId == profile.Id && x.InvitationAccepted == null);
            if (profileOrganisation == null)
            {
                string status = invitationAccepted ? "accept" : "reject";
                throw new ClientError(404, $"Failed to {status} invitation. Profile Organisation not found.");
            }

            profileOrganisation.InvitationAccepted = invitationAccepted;
            await _context.SaveChangesAsync();
        }

        public async Task<AuthResultDto> CreateOrganisation(CreateOrganisationDto payload)
        {
            var currentUserId = _contextAccessorService.GetCurrentUserId();
            if (currentUserId == null) throw new ClientError(401, "Failed to retrieve currently authenticated user");
            var currentUser = await _userManager.FindByIdAsync(currentUserId);
            if (currentUser == null) throw new ClientError(404, "Current User not found");

            var organisation = new Organisation
            {
                Name = payload.Name,
                Description = payload.Description,
                Subdomain = payload.Subdomain,
            };
            if (payload.OrganizationImage != null)
            {
                var uploadedResult = await InitialImageUpload(payload.OrganizationImage, ImageTypes.Avatar);
                organisation.OrganizationImageUrl = uploadedResult["url"];
                organisation.OrganizationImageName = uploadedResult["name"];
            }
            if (payload.OrganizationHeaderImage != null)
            {
                var uploadedResult = await InitialImageUpload(payload.OrganizationHeaderImage, ImageTypes.Header);
                organisation.OrganizationHeaderImageUrl = uploadedResult["url"];
                organisation.OrganizationHeaderImageName = uploadedResult["name"];
            }

            _context.Organisations.Add(organisation);
            await _context.SaveChangesAsync();
            await RegisterAsOrgAdmin(organisation, currentUser);

            //ADDED because using DB Injection threw:
            /*System.AggregateException: 'Some services are not able to be constructed (Error while validating the service descriptor 'ServiceType: BusinessLogicLayer.Interfaces.IProfileService Lifetime: Scoped ImplementationType: BusinessLogicLayer.Services.ProfileService': Unable to resolve service for type 'BusinessLogicLayer.Services.AuthenticationService' while attempting to activate 'BusinessLogicLayer.Services.ProfileService'.)'*/
            using (var scope = _scopeFactory.CreateScope())
            {
                var authService = scope.ServiceProvider.GetRequiredService<IAuthenticationService>();
                await authService.LogoutAsync();
            }

            var newToken = await _tokenService.GenerateJwtToken(currentUser, false, null);
            return newToken;

        }

        private async Task RegisterAsOrgAdmin(Organisation organisation, ApplicationUser currentUser)
        {
            var profileOrganisation = new ProfileOrganisation
            {
                ProfileId = _context.Profiles.First(x => x.UserId == currentUser.Id).Id,
                OrganisationId = organisation.Id,
                InvitationAccepted = true,
                IsOrgAdmin = true,
            };
            await _userManager.AddToRoleAsync(currentUser, UserRoles.OrgAdmin);
            await _context.SaveChangesAsync();
        }

        public async Task<GetOrganisationDto> GetOrganisation(string organizationId)
        {
            var organisation = await _context.Organisations.FirstOrDefaultAsync(x => x.Id == organizationId); 
            if(organisation == null)
                throw new ClientError(404, "Organisation not found");
            var organisationDto = new GetOrganisationDto
            {
                OrganizationId = organizationId,
                Name = organisation.Name,
                Description = organisation.Description,
                OrganizationHeaderImageUrl = organisation.OrganizationHeaderImageUrl,
                OrganizationImageUrl = organisation.OrganizationImageUrl,
                Subdomain = organisation.Subdomain,
            };
            return organisationDto;

        }

        public async Task InviteUserToDomain(string profileId, string organisationId)
        {
            var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.Id == profileId);
            if (profile == null) throw new ClientError(404, "Selected Profile not found");
            _context.ProfileOrganisations.Add(new ProfileOrganisation
            {
                ProfileId = profileId,
                OrganisationId = organisationId,
            });
        }

        private async Task<Dictionary<string, string>> InitialImageUpload(IFormFile formFile, string imageType)
        {
            string profilePictureName;
            string result;
            using (var stream = formFile.OpenReadStream())
            {
                profilePictureName = _r2Service.SanitizeFileName(formFile.FileName, imageType == ImageTypes.Avatar ? FileToUpload.OrganizationImage : FileToUpload.OrganizationHeaderImage);
                result = await _r2Service.UploadFileAsync(stream, profilePictureName);
            }
            return new Dictionary<string, string>
            {
                { "url", result },
                { "name", profilePictureName }
            };
        }

        /*private async Task<Dictionary<string, string>> UploadImage(Organisation org, IFormFile formFile, string imageType)
        {
            var user = profile.User;
            if (user == null) throw new ClientError(404, "User not found.");


            string profilePictureName;
            string result;
            // Implement the logic to upload and save the profile picture
            if (!string.IsNullOrEmpty(profile.ProfilePictureUrl) && !string.IsNullOrEmpty(profile.ProfilePictureName))
            {
                using (var stream = formFile.OpenReadStream())
                {
                    profilePictureName = _r2Service.SanitizeFileName(formFile.FileName, FileToUpload.ProfileImage);
                    result = await _r2Service.UpdateFileAsync(stream, profile.ProfilePictureName, profilePictureName);
                }
                return new Dictionary<string, string>
                {
                    { "url", result },
                    { "name", profilePictureName }
                };
            }
            else
            {
                using (var stream = formFile.OpenReadStream())
                {
                    profilePictureName = _r2Service.SanitizeFileName(formFile.FileName, FileToUpload.ProfileImage);
                    result = await _r2Service.UploadFileAsync(stream, profilePictureName);
                }
                return new Dictionary<string, string>
                {
                    { "url", result },
                    { "name", profilePictureName }
                };
            }
        }*/
    }
}
