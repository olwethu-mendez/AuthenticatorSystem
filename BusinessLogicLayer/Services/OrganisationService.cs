using BusinessLogicLayer.DTOs.Authentication;
using BusinessLogicLayer.DTOs.Organisations;
using BusinessLogicLayer.DTOs.Profile;
using BusinessLogicLayer.DTOs.ProfileOrganisation;
using BusinessLogicLayer.DTOs.User;
using BusinessLogicLayer.DTOs.OrganisationProjects;
using BusinessLogicLayer.Helper;
using BusinessLogicLayer.Infrastructure;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Data;
using DataAccessLayer.Infrastructure;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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
        private readonly ITenantService _tenantService;
        private readonly ISmsService _smsService;
        private readonly IEmailService _emailService;
        private readonly PhoneEmailIdentifierHelper _isPhoneEmailHelper;

        public OrganisationService(ContextAccessorService contextAccessorService, UserManager<ApplicationUser> userManager, R2Service r2Service, ApplicationDbContext context, IServiceScopeFactory serviceScope, TokenService tokenService, ITenantService tenantService, ISmsService smsService, IEmailService emailService, PhoneEmailIdentifierHelper isPhoneEmailHelper)
        {
            _contextAccessorService = contextAccessorService;
            _userManager = userManager;
            _r2Service = r2Service;
            _context = context;
            _scopeFactory = serviceScope;
            _tokenService = tokenService;
            _tenantService = tenantService;
            _smsService = smsService;
            _emailService = emailService;
            _isPhoneEmailHelper = isPhoneEmailHelper;
        }

        public async Task<AuthResultDto> AcceptInvitation(string organisationId, bool invitationAccepted)
        {
            var currentUserId = _contextAccessorService.GetCurrentUserId();
            if (currentUserId == null) throw new ClientError(401, "Failed to retrieve currently authenticated user");
            var profile = await _context.Profiles.Include(x => x.User).FirstOrDefaultAsync(p => p.UserId == currentUserId);
            if (profile == null) throw new ClientError(404, "Current User Profile not found");
            if (profile.User == null) throw new ClientError(404, "Current User not found");

            var profileOrganisation = await _context.ProfileOrganisations.IgnoreQueryFilters().FirstOrDefaultAsync(
                x => x.OrganisationId == organisationId &&
                x.ProfileId == profile.Id &&
                x.InvitationAccepted == null);
            if (profileOrganisation == null)
            {
                string status = invitationAccepted ? "accept" : "reject";
                throw new ClientError(404, $"Failed to {status} invitation. Profile Organisation not found.");
            }

            profileOrganisation.InvitationAccepted = invitationAccepted;
            await _context.SaveChangesAsync();

            using (var scope = _scopeFactory.CreateScope())
            {
                var authService = scope.ServiceProvider.GetRequiredService<IAuthenticationService>();
                await authService.LogoutAsync();
            }

            var newToken = await _tokenService.GenerateJwtToken(profile.User, false, null, organisationId);
            return newToken;
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
                IsPublic = payload.IsPublic ?? false,
                Subdomain = payload.Subdomain ?? MultiTenantHelper.ToSubdomain(payload.Name),
                Status = StatusValue.Activated,
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

            var newToken = await _tokenService.GenerateJwtToken(currentUser, false, null, organisation.Id);
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
            await _context.ProfileOrganisations.AddAsync(profileOrganisation);
            await _userManager.AddToRoleAsync(currentUser, UserRoles.OrgAdmin);
            await _context.SaveChangesAsync();
        }

        public async Task<GetOrganisationDto> GetOrganisation(string organizationId)
        {
            var currentUserId = _contextAccessorService.GetCurrentUserId();
            var profile = await _context.Profiles.FirstOrDefaultAsync(x => x.UserId == currentUserId);
            if (profile == null) throw new ClientError(404, "Current user profile not found");
            var organisation = await _context.Organisations.FirstOrDefaultAsync(x => x.Id == organizationId);
            if (organisation == null)
                throw new ClientError(404, "Organisation not found");
            var profileOrganisation = await _context.ProfileOrganisations
                .Include(x => x.Organisation).ThenInclude(x => x.OrganisationProjects).Include(x => x.Profile).ThenInclude(x => x.User)
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.OrganisationId == organizationId && x.ProfileId == profile.Id && x.InvitationAccepted == true);
            if (profileOrganisation == null) throw new ClientError(403, "You are not authorized to see this organisation");
            if ((organisation.Status == StatusValue.Activated && profileOrganisation.OrganisationId == organizationId) || organisation.IsPublic)
            {
                var organisationDto = new GetOrganisationDto
                {
                    OrganizationId = organizationId,
                    Name = organisation.Name,
                    Description = organisation.Description,
                    IsPublic = organisation.IsPublic,
                    InvitationAccepted = profileOrganisation.InvitationAccepted,
                    IsAdmin = profileOrganisation.IsOrgAdmin,
                    Projects = organisation.OrganisationProjects?.Select(p => new GetProjectsDto
                    {
                        ProjectId = p.Id,
                        Name = p.Name,
                        ProjectImageUrl = p.ProjectImageUrl
                    }).ToList() ?? new List<GetProjectsDto>(),
                    OrganizationHeaderImageUrl = organisation.OrganizationHeaderImageUrl,
                    OrganizationImageUrl = organisation.OrganizationImageUrl,
                    Subdomain = organisation.Subdomain,
                    Status = organisation.Status,
                };
                return organisationDto;
            }
            throw new ClientError(403, "You are not authorized to see this organisation");
        }

        public async Task<GetOrganisationDto> GetCurrentOrganisation()
        {
            var organisationId = _tenantService.TenantId;
            if (organisationId == null) throw new ClientError(400, "Organisation not found or invalid");
            return await GetOrganisation(organisationId);
        }

        public async Task<List<GetOrganisationsDto>> GetOrganisations()
        {
            var currentUserId = _contextAccessorService.GetCurrentUserId();
            if (currentUserId == null)
                throw new ClientError(401, "Failed to retrieve currently authenticated user");

            var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == currentUserId);

            if (profile == null) throw new ClientError(404, "Current User Profile not found");

            var organisations = await _context.ProfileOrganisations
                .Where(po => po.ProfileId == profile.Id && (po.InvitationAccepted == true || po.InvitationAccepted == null) && po.Organisation!.Status == StatusValue.Activated)
                .Include(x => x.Organisation).Include(x => x.Profile)
                .IgnoreQueryFilters()
                .Select(po => new GetOrganisationsDto
                {
                    OrganizationId = po.Organisation.Id,
                    Name = po.Organisation.Name,
                    Subdomain = po.Organisation.Subdomain,
                    OrganizationImageUrl = po.Organisation.OrganizationImageUrl,
                    IsPublic = po.Organisation.IsPublic,
                    Status = po.Organisation.Status,
                    InvitationAccepted = po.InvitationAccepted,
                    IsAdmin = po.IsOrgAdmin
                })
                .ToListAsync();

            return organisations;
        }

        public async Task<List<GetMyOrganisationDto>> GetMyOrganisations()
        {
            var currentUserId = _contextAccessorService.GetCurrentUserId();
            if (currentUserId == null) throw new ClientError(401, "Failed to retrieve currently authenticated user");

            var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == currentUserId);

            if (profile == null) throw new ClientError(404, "Current User Profile not found");

            var organisations = await _context.ProfileOrganisations
                .Where(po => po.ProfileId == profile.Id && po.IsOrgAdmin == true)
                .Include(x => x.Organisation).Include(x => x.Profile)
                .IgnoreQueryFilters()
                .Select(po => new GetMyOrganisationDto
                {
                    OrganizationId = po.Organisation.Id,
                    Name = po.Organisation.Name,
                    Subdomain = po.Organisation.Subdomain,
                    OrganizationImageUrl = po.Organisation.OrganizationImageUrl,
                    IsPublic = po.Organisation.IsPublic,
                    IsAdmin = po.IsOrgAdmin,
                    Status = po.Organisation.Status,
                })
                .ToListAsync();

            return organisations;
        }

        public async Task<List<GetOrganisationsDto>> GetPublicOrganisations()
        {
            var currentUserId = _contextAccessorService.GetCurrentUserId();
            if (currentUserId == null)
                throw new ClientError(401, "Failed to retrieve currently authenticated user");

            var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == currentUserId);

            if (profile == null) throw new ClientError(404, "Current User Profile not found");

            var organisations = await _context.ProfileOrganisations
                .Where(po => po.ProfileId == profile.Id && po.Organisation!.IsPublic == true && po.Organisation!.Status == StatusValue.Activated)
                .IgnoreQueryFilters()
                .Select(po => new GetOrganisationsDto
                {
                    OrganizationId = po.Organisation.Id,
                    Name = po.Organisation.Name,
                    Subdomain = po.Organisation.Subdomain,
                    OrganizationImageUrl = po.Organisation.OrganizationImageUrl,
                    IsPublic = po.Organisation.IsPublic,
                    InvitationAccepted = po.InvitationAccepted,
                    IsAdmin = po.IsOrgAdmin,
                    Status = po.Organisation.Status
                })
                .ToListAsync();

            return organisations;
        }

        public async Task<List<GetProfileOrganisationDto>> GetOrganisationMembers(string organisationId)
        {
            // 1. Validate the user has access to this org first
            await ValidateTenantAccess();

            // 2. Fetch all ProfileOrganisation records for this Org
            var members = await _context.ProfileOrganisations
                .Include(po => po.Profile)
                .ThenInclude(p => p.User) // Get the identity data too
                .Include(po => po.Organisation)
                .ThenInclude(o => o.OrganisationProjects)
                .Where(po => po.OrganisationId == organisationId && po.InvitationAccepted == true)
                .ToListAsync();

            // 3. Map to DTO
            return members.Select(m => new GetProfileOrganisationDto
            {
                Profile = new GetUsersListDto
                {
                    UserId = m.Profile.UserId,
                    ProfileId = m.Profile.Id,
                    FirstName = m.Profile.FirstName,
                    LastName = m.Profile.LastName,
                    Username = m.Profile.User.UserName,
                    ProfilePictureUrl = m.Profile.ProfilePictureUrl
                },
                Organisation = new GetOrganisationDto
                {
                    OrganizationId = m.OrganisationId,
                    Name = m.Organisation.Name,
                    Description = m.Organisation.Description,
                    IsPublic = m.Organisation.IsPublic,
                    OrganizationHeaderImageUrl = m.Organisation.OrganizationHeaderImageUrl,
                    OrganizationImageUrl = m.Organisation.OrganizationImageUrl,
                    Status = m.Organisation.Status,
                    InvitationAccepted = m.InvitationAccepted,
                    IsAdmin = m.IsOrgAdmin,
                    Subdomain = m.Organisation.Subdomain,
                    Projects = m.Organisation.OrganisationProjects?.Select(p => new GetProjectsDto
                    {
                        ProjectId = p.Id,
                        Name = p.Name,
                        ProjectImageUrl = p.ProjectImageUrl
                    }).ToList() ?? new List<GetProjectsDto>()
                },
                IsOrgAdmin = m.IsOrgAdmin ?? false,
                InvitationAccepted = m.InvitationAccepted
            }).ToList();
        }

        public async Task InviteUserToDomain(string profileId)
        {
            await ValidateTenantAccess(requireAdmin: true);

            var tenantId = _tenantService.TenantId;
            if (tenantId == null) throw new ClientError(403, "Please switch to an organisation to access it.");
            var organisation = await _context.Organisations.FirstOrDefaultAsync(x => x.Id == tenantId);
            if (organisation == null) throw new ClientError(404, "This organisation is not found.");

            var profile = await _context.Profiles
                .Include(x => x.User)
                .FirstOrDefaultAsync(p => p.Id == profileId && p.User!.IsDeactivated == false && p.User.IsDeactivatedByAdmin == false);

            if (profile == null || profile.User == null)
                throw new ClientError(404, "Profile not found.");

            var contactDetails = _isPhoneEmailHelper.TypeIdentifier(profile.User?.UserName ?? "");
            if (contactDetails.contactType == ContactType.Email)
            {
                if (contactDetails.contactValue != null)
                    await SendInvitationByEmailAsync(organisation.Name, contactDetails.contactValue!);
                else
                    throw new ClientError(400, "Failed to invite user. Invalid contact details provided.");
            }
            if (contactDetails.contactType == ContactType.Phone)
            {
                if (contactDetails.contactValue != null)
                    await SendInvitationBySmsAsync(organisation.Name, profile.User?.CountryCode!, contactDetails.contactValue!);
                else
                    throw new ClientError(400, "Failed to invite user. Invalid contact details provided.");
            }
            if (contactDetails.contactType == ContactType.Invalid)
                throw new ClientError(400, "Failed to invite user. Invalid contact details provided.");

            _context.ProfileOrganisations.Add(new ProfileOrganisation
            {
                ProfileId = profileId,
                OrganisationId = tenantId!,
                InvitationAccepted = null,
                IsOrgAdmin = false,
            });

            await _context.SaveChangesAsync();
        }

        public async Task<AuthResultDto> SwitchOrganisation(string organisationId)
        {
            var userId = _contextAccessorService.GetCurrentUserId();

            if (userId == null)
                throw new ClientError(401, "User not authenticated");

            var currentUser = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);

            var isMember = await _context.ProfileOrganisations
                .Include(x => x.Profile).Include(x => x.Organisation)
                .IgnoreQueryFilters()
                .AnyAsync(po =>
                    po.Profile.UserId == userId &&
                    po.OrganisationId == organisationId &&
                    po.InvitationAccepted == true &&
                    po.Organisation.Status == StatusValue.Activated);

            if (!isMember)
                throw new ClientError(403, "You are not a member of this organisation or is no longer available.");

            var newToken = await _tokenService.GenerateJwtToken(currentUser, false, null, organisationId);

            return newToken;
        }

        public async Task<OrganisationStatus> ChangeOrganisationStatus(string organisationId)
        {
            var userId = _contextAccessorService.GetCurrentUserId();
            if (userId == null) throw new ClientError(401, "User not authenticated");
            var currentUserProfile = await _context.Profiles.Include(x => x.User).FirstOrDefaultAsync(x => x.UserId == userId);
            if (currentUserProfile == null) throw new ClientError(404, "User profile not found for logged in user");

            var organisation = await _context.ProfileOrganisations
                .Include(x => x.Organisation).Include(x => x.Profile)
                .Where(po => po.ProfileId == currentUserProfile.Id && po.IsOrgAdmin == true && po.OrganisationId == organisationId && po.Organisation.Status == StatusValue.Activated)
                .Select(po => po.Organisation).FirstOrDefaultAsync();
            if (organisation == null) throw new ClientError(400, "organisation either not found or you are not allowed to make allowed to manage it.");

            if (organisation.Status == StatusValue.Activated) organisation.Status = StatusValue.Deactivated;
            else if (organisation.Status == StatusValue.Deactivated) organisation.Status = StatusValue.Activated;
            else throw new ClientError(403, $"Your current status is {organisation.Status}. You are not allowed to change this.");

            await _context.SaveChangesAsync();
            return new OrganisationStatus { Status = organisation.Status };
        }

        public Task UpdateOrganisation(UpdateProfileDto payload)
        {
            throw new NotImplementedException();
        }

        public Task UpdateOrganisationLogo(IFormFile? picture)
        {
            throw new NotImplementedException();
        }

        public Task UpdateOrganisationHeader(IFormFile? picture)
        {
            throw new NotImplementedException();
        }

        private async Task<Dictionary<string, string>> InitialImageUpload(IFormFile formFile, string imageType)
        {
            string profilePictureName;
            string result;
            using (var stream = formFile.OpenReadStream())
            {
                profilePictureName = _r2Service.SanitizeFileName(
                    formFile.FileName,
                    imageType == ImageTypes.Avatar ? FileToUpload.OrganizationImage : FileToUpload.OrganizationHeaderImage);
                result = await _r2Service.UploadFileAsync(stream, profilePictureName);
            }
            return new Dictionary<string, string>
            {
                { "url", result },
                { "name", profilePictureName }
            };
        }

        private async Task<ProfileOrganisation> ValidateTenantAccess(bool requireAdmin = false)
        {
            var tenantId = _tenantService.TenantId;
            if (string.IsNullOrEmpty(tenantId))
                throw new ClientError(400, "Tenant header required.");

            var currentUserId = _contextAccessorService.GetCurrentUserId();
            if (currentUserId == null)
                throw new ClientError(401, "User not authenticated.");

            var membership = await _context.ProfileOrganisations
                .Include(po => po.Profile)
                .FirstOrDefaultAsync(po =>
                    po.OrganisationId == tenantId &&
                    po.Profile.UserId == currentUserId &&
                    po.InvitationAccepted == true);

            if (membership == null)
                throw new ClientError(403, "Access denied to this organisation.");

            if (requireAdmin && membership.IsOrgAdmin != true)
                throw new ClientError(403, "Admin privileges required.");

            return membership;
        }



        private async Task SendInvitationBySmsAsync(string organisationName, string countryCode, string phoneNumber)
        {
            var message = $"Hi,\nYou have been invited to join the organisation: {organisationName} on the Authenticator System app";
            await _smsService.SendSmsAsync($"{countryCode}{phoneNumber}", message);
        }

        private async Task SendInvitationByEmailAsync(string organisationName, string email)
        {
            var message = $@"
                <div style='font-family: Arial, sans-serif;'>
                    <h2>Organisation Invitation</h2>
                    <p>You have been invited to join the organisation: {organisationName}.</p>
                    <p>Login into the platform to accept the invitation.</p>
                </div>";

            await _emailService.SendEmailAsync(email, $"Invitation to Organisation: {organisationName}", message);
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
