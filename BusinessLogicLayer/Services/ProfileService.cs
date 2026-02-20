using BusinessLogicLayer.DTOs.Authentication;
using BusinessLogicLayer.DTOs.Profile;
using BusinessLogicLayer.Infrastructure;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Data;
using DataAccessLayer.Infrastructure;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySqlX.XDevAPI.Common;

namespace BusinessLogicLayer.Services
{
    public class ProfileService : IProfileService
    {
        private readonly TokenService _tokenService; // Inject this!
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IAuthenticationService _authService;
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ContextAccessorService _contextAccessor;
        private readonly R2Service _r2Service;
        private readonly ISmsService _smsService;
        private readonly IEmailService _emailService;

        public ProfileService(
            TokenService tokenService,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context,
            IAuthenticationService authService,
            IConfiguration configuration,
            IServiceScopeFactory serviceScope,
            ContextAccessorService contextAccessor,
            R2Service r2Service,
            ISmsService smsService,
            IEmailService emailService)
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _authService = authService;
            _configuration = configuration;
            _scopeFactory = serviceScope;
            _contextAccessor = contextAccessor;
            _r2Service = r2Service;
            _smsService = smsService;
            _emailService = emailService;
        }

        public async Task<AuthResultDto> CreateProfile(CreateProfileDto payload)
        {
            var userId = _contextAccessor.GetCurrentUserId();
            if (userId == null) throw new ClientError(401, "User is not authenticated.");

            var profile = new Profile
            {
                UserId = userId,
                FirstName = payload.FirstName,
                LastName = payload.LastName,
                Gender =  payload.Gender,
                GenderSelfDescription = payload.GenderSelfDescription,
                
            };
            if (payload.ProfilePicture != null)
            {
                var uploadResult = await InitialProfilePictureUpload(payload.ProfilePicture);
                profile.ProfilePictureUrl = uploadResult["url"];
                profile.ProfilePictureName = uploadResult["name"];
            }

            _context.Profiles.Add(profile);
            await _context.SaveChangesAsync();

            //ADDED because using DB Injection threw:
            /*System.AggregateException: 'Some services are not able to be constructed (Error while validating the service descriptor 'ServiceType: BusinessLogicLayer.Interfaces.IProfileService Lifetime: Scoped ImplementationType: BusinessLogicLayer.Services.ProfileService': Unable to resolve service for type 'BusinessLogicLayer.Services.AuthenticationService' while attempting to activate 'BusinessLogicLayer.Services.ProfileService'.)'*/
            using (var scope = _scopeFactory.CreateScope())
            {
                var authService = scope.ServiceProvider.GetRequiredService<IAuthenticationService>();
                await authService.LogoutAsync();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new ClientError(401, "User not found.");
            var newToken = await _tokenService.GenerateJwtToken(user, payload.StayLoggedIn, null);
            return newToken;
        }

        public async Task DeactivateProfile(DeactivateAccountDto payload)
        {
            var userId = _contextAccessor.GetCurrentUserId();
            if (userId == null) throw new ClientError(401, "User is not authenticated.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new ClientError(404, "User not found.");

            if (user.IsDeactivated) throw new ClientError(400, "Profile already deactivated");
            if (user.IsDeactivatedByAdmin) throw new ClientError(400, "This account is already banned");
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, payload.Password);
            if (!isPasswordValid) throw new ClientError(400, "Invalid password. Cannot deactivate account.");
            user.IsDeactivated = true;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) throw new ClientError(500, "Failed to update lockout status.");
        }

        public async Task<AuthResultDto> ActivateProfile(DeactivateAccountDto payload)
        {
            var userId = _contextAccessor.GetCurrentUserId();
            if (userId == null) throw new ClientError(401, "User is not authenticated.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new ClientError(404, "User not found.");

            if(!user.IsDeactivated) throw new ClientError(400, "Profile already activated");
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, payload.Password);
            if (!isPasswordValid) throw new ClientError(400, "Invalid password. Cannot activate account.");
            if (user.IsDeactivatedByAdmin) throw new ClientError(403, "Account banned by admin. Contact support to reactivate.");
            user.IsDeactivated = false;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) throw new ClientError(500, "Failed to update lockout status.");

            return await _tokenService.GenerateJwtToken(user, false, null);
        }

        public async Task<GetProfileDto> GetProfile()
        {
            var userId = _contextAccessor.GetCurrentUserId();
            if (userId == null) throw new ClientError(401, "User is not authenticated.");
            var profile = await _context.Profiles.Include(x => x.User).FirstOrDefaultAsync(x => x.UserId == userId);
            if (profile == null) throw new ClientError(404, "Profile not found.");
            var lastPassword = await _context.PreviousPasswords.Where(x=>x.UserId == userId).OrderByDescending(x=>x.DateSet).FirstOrDefaultAsync();
            var userProfile = new GetProfileDto
            {
                UserId = profile?.User?.Id,
                ProfileId = profile?.Id,
                Username = profile?.User?.UserName,
                FirstName = profile?.FirstName,
                LastName = profile?.LastName,
                EmailAddress = profile?.User?.Email,
                EmailConfirmed = profile?.User?.EmailConfirmed,
                CountryCode = !string.IsNullOrEmpty(profile?.User?.CountryCode) ? $"{profile?.User?.CountryCode}" : null,
                PhoneNumber = !string.IsNullOrEmpty(profile?.User?.PhoneNumber) ? $"{profile?.User?.PhoneNumber}" : null,
                PhoneNumberConfirmed = profile?.User?.PhoneNumberConfirmed,
                Gender = profile?.Gender,
                GenderSelfDescription = profile?.GenderSelfDescription,
                CreatedAt = profile?.User?.CreatedAt,
                ProfilePictureUrl =  profile?.ProfilePictureUrl,
                ProfilePictureName = profile?.ProfilePictureName,
                PasswordLastUpdated = lastPassword != null ? (int)(DateTime.UtcNow - lastPassword.DateSet).TotalDays : null,
            };
            return userProfile;
        }

        public async Task UpdateProfile(UpdateProfileDto payload)
        {
            var userId = _contextAccessor.GetCurrentUserId();
            if (userId == null) throw new ClientError(401, "User is not authenticated.");

            var profile = await _context.Profiles.Where(x => x.UserId == userId).Include(x => x.User).FirstOrDefaultAsync();
            if (profile == null) throw new ClientError(404, "Profile not found.");

            profile.FirstName = string.IsNullOrEmpty(payload.FirstName) ? profile.FirstName : payload.FirstName;
            profile.LastName = string.IsNullOrEmpty(payload.LastName) ? profile.LastName : payload.LastName;
            profile.Gender = string.IsNullOrEmpty(payload.Gender) ? profile.Gender : payload.Gender;
            profile.GenderSelfDescription = string.IsNullOrEmpty(payload.GenderSelfDescription) ? profile.GenderSelfDescription : payload.GenderSelfDescription;

            await _context.SaveChangesAsync();
            return;
        }

        public async Task UpdateProfilePicture(IFormFile? profilePicture)
        {
            var userId = _contextAccessor.GetCurrentUserId();
            if (userId == null) throw new ClientError(401, "User is not authenticated.");

            var profile = await _context.Profiles.Where(x => x.UserId == userId).Include(x => x.User).FirstOrDefaultAsync();
            if (profile == null) throw new ClientError(404, "Profile not found.");

            if (profilePicture != null)
            {
                var uploadResult = await UploadProfilePicture(profile, profilePicture);
                profile.ProfilePictureUrl = uploadResult["url"];
                profile.ProfilePictureName = uploadResult["name"];
            }
            else
            {
                throw new ClientError(400, "Remove profile picture feature coming soon");
            }
            await _context.SaveChangesAsync();
            return;
        }

        public async Task<Dictionary<string, string>> InitialProfilePictureUpload(IFormFile formFile)
        {
            string profilePictureName;
            string result;
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

        public async Task<Dictionary<string, string>> UploadProfilePicture(Profile profile, IFormFile formFile)
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
        }

        public async Task UpdateProfileEmail(UpdateEmailDto payload)
        {
            if (string.IsNullOrEmpty(payload.NewEmail)) throw new ClientError(400, "Email address is required");
            var userId = _contextAccessor.GetCurrentUserId();
            if (userId == null) throw new ClientError(401, "User is not authenticated.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new ClientError(404, "User not found.");

            // 1. Security Check: Verify Password
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, payload.Password);
            if (!isPasswordValid) throw new ClientError(400, "Invalid password. Cannot change email.");

            // 2. Availability Check: Is email taken?
            var existingUser = await _userManager.FindByEmailAsync(payload.NewEmail);
            if (existingUser != null && existingUser.Id != userId)
                throw new ClientError(400, "Email address is already in use.");

            // 3. Update Email (Set as unconfirmed until OTP/Link is verified)
            user.Email = payload.NewEmail;
            user.EmailConfirmed = false;
            user.UserName = user.UserName; // Keep username same for now or update if needed

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) throw new ClientError(500, "Failed to update email.");

            // 4. Trigger Verification
            await SendEmailOtpAsync(user);
        }

        // 1. Logic for the "Verify" badge next to Email in Flutter
        public async Task ConfirmUnconfirmedProfileEmail()
        {
            var userId = _contextAccessor.GetCurrentUserId();
            if (userId == null) throw new ClientError(401, "User is not authenticated.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new ClientError(404, "User not found.");

            if (user.EmailConfirmed) throw new ClientError(400, "Email is already verified.");

            // This triggers the 6-digit code via the method you already wrote
            await SendEmailOtpAsync(user);
        }

        // 2. Logic for the "Verify" badge next to Phone in Flutter
        public async Task ConfirmUnconfirmedProfilePhoneNumber()
        {
            var userId = _contextAccessor.GetCurrentUserId();
            var user = await _userManager.FindByIdAsync(userId!);
            if (user == null) throw new ClientError(404, "User not found.");

            if (user.PhoneNumberConfirmed) throw new ClientError(400, "Phone number is already verified.");
            if (string.IsNullOrEmpty(user.PhoneNumber)) throw new ClientError(400, "No phone number found to verify.");

            await SendOtpSmsAsync(user);
        }

        public async Task UpdateProfilePhoneNumber(UpdatePhoneNumberDto payload)
        {
            if (string.IsNullOrEmpty(payload.NewPhoneNumber)) throw new ClientError(400, "Phone number required.");
            var userId = _contextAccessor.GetCurrentUserId();
            var user = await _userManager.FindByIdAsync(userId!);
            if (user == null) throw new ClientError(404, "User not found.");

            // Security: Check Password
            if (!await _userManager.CheckPasswordAsync(user, payload.Password))
                throw new ClientError(400, "Invalid password.");

            if (user.PhoneNumber == payload.NewPhoneNumber) throw new ClientError(400, "Phone number already in use");

            if (string.IsNullOrEmpty(payload.CountryCode) && string.IsNullOrEmpty(user.CountryCode)) throw new ClientError(400, "Country code not provided");
            user.CountryCode = payload.CountryCode != null ? payload.CountryCode : user.CountryCode;
            user.PhoneNumber = payload.NewPhoneNumber;
            user.PhoneNumberConfirmed = false;
            user.UserName = user.UserName; // Keep username same for now or update if needed

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) throw new ClientError(500, "Failed to update phone number.");

            // Trigger SMS OTP
            await SendOtpSmsAsync(user);
        }

        public async Task SetPreferredContactMode(string mode) // mode = "email" or "phone"
        {
            var userId = _contextAccessor.GetCurrentUserId();
            var user = await _userManager.FindByIdAsync(userId!);

            if (mode.ToLower() == "email")
            {
                if (!user!.EmailConfirmed) throw new ClientError(400, "Verify email before setting as primary.");
                user.UserName = user.Email; // Set Email as Login Username
            }
            else if (mode.ToLower() == "phone")
            {
                if (!user!.PhoneNumberConfirmed) throw new ClientError(400, "Verify phone before setting as primary.");
                user.UserName = user.PhoneNumber; // Set Phone as Login Username
            }

            await _userManager.UpdateAsync(user!);
        }

        public async Task ChangePassword(ChangePasswordDto payload)
        {
            var userId = _contextAccessor.GetCurrentUserId();
            var user = await _userManager.FindByIdAsync(userId!);

            var result = await _userManager.ChangePasswordAsync(user!, payload.CurrentPassword, payload.NewPassword);

            if (!result.Succeeded)
            {
                var error = result.Errors.FirstOrDefault()?.Description ?? "Password change failed.";
                throw new ClientError(400, error);
            }
        }

        private async Task SendOtpSmsAsync(ApplicationUser user)
        {
            if (user == null) throw new ClientError(404, "Failed to generate OTP. User not found");
            var otpGen = await _userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber!);
            var message = $"Hi,\nThank you for registering with our service.\nYour OTP code is: {otpGen}";
            await _smsService.SendSmsAsync($"{user.CountryCode}{user.PhoneNumber}", message);
        }

        private async Task SendEmailOtpAsync(ApplicationUser user)
        {
            // Generate a 6-digit numeric code using the "Email" provider
            // Note: This uses the built-in Identity logic for email-based tokens
            var otpCode = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");

            var message = $@"
        <div style='font-family: Arial, sans-serif;'>
            <h2>Verify your new email</h2>
            <p>You have updated your email. Please use the following code to verify your new email address:</p>
            <h1 style='letter-spacing: 5px; color: #4A90E2;'>{otpCode}</h1>
            <p>This code will expire in 15 minutes.</p>
        </div>";

            await _emailService.SendEmailAsync(user.Email!, "Your Verification Code", message);
        }

        private TimeSpan LastChanged(DateTime dateFrom)
        {
            return DateTime.UtcNow - dateFrom;
        }
    }
}
