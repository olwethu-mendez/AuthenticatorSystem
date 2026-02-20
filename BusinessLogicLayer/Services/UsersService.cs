using BusinessLogicLayer.DTOs.Authentication;
using BusinessLogicLayer.DTOs.Profile;
using BusinessLogicLayer.DTOs.User;
using BusinessLogicLayer.Hubs;
using BusinessLogicLayer.Infrastructure;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Data;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class UsersService : IUsersService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ContextAccessorService _contextAccessor;
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<UserHub> _hubContext;
        private readonly IProfileService _profileService;
        private readonly ISmsService _smsService;
        private readonly IEmailService _emailService;
        public UsersService(UserManager<ApplicationUser> userManager, ContextAccessorService contextAccessor, ApplicationDbContext context, IHubContext<UserHub> hubContext, IProfileService profileService, ISmsService smsService, IEmailService emailService)
        {
            _userManager = userManager;
            _contextAccessor = contextAccessor;
            _context = context;
            _hubContext = hubContext;
            _profileService = profileService;
            _smsService = smsService;
            _emailService = emailService;
        }
        public async Task<List<GetUsersListDto>> GetAllUsers()
        {
            var userId = _contextAccessor.GetCurrentUserId();
            if (userId == null) throw new ClientError(401, "User not authenticated");
            var currentUser = await _userManager.FindByIdAsync(userId);
            if (currentUser == null) throw new ClientError(400, "Invalid user provided. User not found");
            var userRoles = await _userManager.GetRolesAsync(currentUser);
            if (!userRoles.Contains(UserRoles.Admin)) throw new ClientError(403, "Only admins can perform this function");

            return await (from user in _context.Users
                              // This is a Left Join: User is the root, Profile is optional
                          join profile in _context.Profiles on user.Id equals profile.UserId into profileGroup
                          from userProfile in profileGroup.DefaultIfEmpty()

                              // If you are filtering for a specific user, uncomment the next line:
                              // where user.Id == userId 

                          select new GetUsersListDto()
                          {
                              UserId = user.Id,
                              ProfileId = userProfile.Id,
                              FirstName = userProfile != null ? userProfile.FirstName : "N/A",
                              LastName = userProfile != null ? userProfile.LastName : "N/A",
                              Username = user.UserName,
                              EmailConfirmed = user.EmailConfirmed,
                              PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                              IsDeactivated = user.IsDeactivated,
                              IsDeactivatedByAdmin = user.IsDeactivatedByAdmin,
                              ProfilePictureUrl = userProfile != null ? userProfile.ProfilePictureUrl : null,
                          }).ToListAsync();
        }

        public async Task<GetUserDto?> GetSingleUser(string userId)
        {
            var currentUserId = _contextAccessor.GetCurrentUserId();
            if (currentUserId == null) throw new ClientError(401, "User not authenticated");
            var currentUser = await _userManager.FindByIdAsync(currentUserId);
            if (currentUser == null) throw new ClientError(400, "Invalid user provided. User not found");
            var userRoles = await _userManager.GetRolesAsync(currentUser);
            if (!userRoles.Contains(UserRoles.Admin)) throw new ClientError(403, "Only admins can perform this function");

            return await (from user in _context.Users
                              // This is a Left Join: User is the root, Profile is optional
                          join profile in _context.Profiles on user.Id equals profile.UserId into profileGroup
                          from userProfile in profileGroup.DefaultIfEmpty()

                              // If you are filtering for a specific user, uncomment the next line:
                              // where user.Id == userId 

                          select new GetUserDto()
                          {
                              UserId = user.Id,
                              ProfileId = userProfile.Id,
                              FirstName = userProfile != null ? userProfile.FirstName : "N/A",
                              LastName = userProfile != null ? userProfile.LastName : "N/A",
                              Username = user.UserName,
                              EmailAddress = user.Email,
                              EmailConfirmed = user.EmailConfirmed,
                              PhoneNumber =  !string.IsNullOrEmpty(user.PhoneNumber) ? user.CountryCode + user.PhoneNumber : null,
                              PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                              IsDeactivated = user.IsDeactivated,
                              IsDeactivatedByAdmin = user.IsDeactivatedByAdmin,
                              DeactivatedAt = user.DeactivatedAt,
                              ProfilePictureUrl = userProfile != null ? userProfile.ProfilePictureUrl : null,
                              CreatedAt = user.CreatedAt
                          }).FirstOrDefaultAsync(x=>x.UserId == userId);
        }

        public async Task<string> AdminDeactivatesUser(string accountUserId)
        {
            var userId = _contextAccessor.GetCurrentUserId();
            if (userId == null) throw new ClientError(401, "User is not authenticated.");
            var userRole = _contextAccessor.GetUserRoles();
            if (userRole == null) throw new ClientError(403, "User roles not found.");
            if (!userRole.Contains(UserRoles.Admin)) throw new ClientError(403, "Only admins can perform this action.");

            if (accountUserId == userId) throw new ClientError(400, "Admins cannot deactivate their own accounts.");

            var accountUser = await _userManager.FindByIdAsync(accountUserId);
            if (accountUser == null) throw new ClientError(404, "Account user not found.");

            accountUser.IsDeactivated = !accountUser.IsDeactivated;

            if (accountUser.IsDeactivated)
            {
                accountUser.IsDeactivatedByAdmin = true;
                var result = await _userManager.UpdateAsync(accountUser);
                if (!result.Succeeded) throw new ClientError(500, "Failed to deactivate account.");
                await _hubContext.Clients.User(accountUserId).SendAsync("ReceiveAccountStatus", new
                {
                    userId = accountUserId, // ADD THIS LINE
                    isDeactivated = accountUser.IsDeactivated,
                    message = "Your account status has been changed by an admin."
                });
                return "deactivated";
            }
            else
            {
                accountUser.IsDeactivatedByAdmin = false;
                var result = await _userManager.UpdateAsync(accountUser);
                if (!result.Succeeded) throw new ClientError(500, "Failed to activate account.");
                return "activated";
            }
        }

        public async Task CreateUser(CreateUserDto payload)
        {
            var userId = _contextAccessor.GetCurrentUserId();
            if (userId == null) throw new ClientError(401, "User is not authenticated.");
            var currentUser = await _userManager.FindByIdAsync(userId);
            if (currentUser == null) throw new ClientError(400, "User not found or authenticated");
            var userRoles = await _userManager.GetRolesAsync(currentUser);
            if (userRoles.Contains(UserRoles.Admin) == false) throw new ClientError(403, "User not authorized to perform this function.");
            if (string.IsNullOrEmpty(payload.Email) && string.IsNullOrEmpty(payload.PhoneNumber))
                throw new ClientError(400, "at least an email or phone number must be provided");
            if (!string.IsNullOrEmpty(payload.Email) && !string.IsNullOrEmpty(payload.PhoneNumber) && payload.PrefersEmail == null)
                throw new ClientError(400, "If both phone and email are provided user must select their preferred mode of communication");

            if (!string.IsNullOrEmpty(payload.Email))
            {
                var userEmailExists = await _userManager.FindByEmailAsync(payload.Email);
                if (userEmailExists != null)
                {
                    throw new ClientError(400, "Email is already registered.");
                }
            }
            if (!string.IsNullOrEmpty(payload.PhoneNumber))
            {
                var userPhoneExists = await _userManager.Users.FirstOrDefaultAsync(u => u.CountryCode == payload.CountryCode && u.PhoneNumber == payload.PhoneNumber);
                if (userPhoneExists != null)
                {
                    throw new ClientError(400, "Phone number is already registered.");
                }
            }

            string preferredCommunication;
            if (!string.IsNullOrEmpty(payload.Email?.Trim()) && !string.IsNullOrEmpty(payload.PhoneNumber?.Trim()))
            {
                if (payload.PrefersEmail == true) preferredCommunication = payload.Email;
                else if (payload.PrefersEmail == false) preferredCommunication = payload.PhoneNumber;
                else preferredCommunication = payload.Email;
            }
            else if (string.IsNullOrEmpty(payload.Email) && !string.IsNullOrEmpty(payload.PhoneNumber))
            {
                preferredCommunication = payload.PhoneNumber;
            }
            else
            {
                preferredCommunication = payload.Email!;
            }

            ApplicationUser user = new ApplicationUser()
            {
                UserName = preferredCommunication,
                Email = string.IsNullOrWhiteSpace(payload.Email) ? null : payload.Email,
                PhoneNumber = string.IsNullOrWhiteSpace(payload.PhoneNumber) ? null : payload.PhoneNumber,
                CountryCode = string.IsNullOrWhiteSpace(payload.CountryCode) ? null : payload.CountryCode,
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
            };
            var password = GenerateSecurePassword(12);
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                var previousPassword = new PreviousPasswords()
                {
                    UserId = user.Id,
                    PasswordHash = user.PasswordHash ?? throw new ClientError(400, "Invalid password"),
                    DateSet = DateTime.UtcNow
                };

                var profile = new Profile
                {
                    UserId = user.Id,
                    FirstName = payload.FirstName,
                    LastName = payload.LastName,
                    Gender = payload.Gender,
                    GenderSelfDescription = payload.GenderSelfDescription,

                };
                if (payload.ProfilePicture != null)
                {
                    var uploadResult = await _profileService.InitialProfilePictureUpload(payload.ProfilePicture);
                    profile.ProfilePictureUrl = uploadResult["url"];
                    profile.ProfilePictureName = uploadResult["name"];
                }

                _context.Profiles.Add(profile);

                await _userManager.AddToRoleAsync(user, UserRoles.General);
                await _context.PreviousPasswords.AddAsync(previousPassword);
                await _context.SaveChangesAsync();
                if (payload.PrefersEmail == true)
                {
                    DateTime dateOfGeneration = DateTime.UtcNow;
                    // Generate a 6-digit numeric code using the "Email" provider
                    // Note: This uses the built-in Identity logic for email-based tokens
                    var otpCode = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");

                    var message = $@"
        <div style='font-family: Arial, sans-serif;'>
            <h2>Verify your email</h2>
            <p>An account has been created for you at our service. Please use the following code to verify your email address:</p>
            <h1 style='letter-spacing: 5px; color: #4A90E2;'>{otpCode}</h1>
            <p>This code will expire in 15 minutes, and your password is: {password}.</p>
            <p>If this is not true, please ignore or report to that an account of your details was created using Admin account {currentUser.UserName} on this date: {dateOfGeneration}.</p>
        </div>";

                    await _emailService.SendEmailAsync(user.Email!, "Your Verification Code", message);
                }
                else if (payload.PrefersEmail == false)
                {
                    DateTime dateOfGeneration = DateTime.UtcNow;
                    var otpGen = await _userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber!);
                    var message = $"Hi,\nAn account has been created for you at our service. If this is not true, please ignore or report to that an account of your details was created using Admin account {currentUser.UserName} on this date: {dateOfGeneration}\nOtherwise, if this is valid, your OTP code is: {otpGen}, your username is {user.UserName} and your password is: {password}";
                    await _smsService.SendSmsAsync($"{user.CountryCode}{user.PhoneNumber}", message);
                }
                else
                {
                    if (user.Email != null && user.PhoneNumber == null)
                    {
                        DateTime dateOfGeneration = DateTime.UtcNow;
                        // Generate a 6-digit numeric code using the "Email" provider
                        // Note: This uses the built-in Identity logic for email-based tokens
                        var otpCode = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");

                        var message = $@"
        <div style='font-family: Arial, sans-serif;'>
            <h2>Verify your email</h2>
            <p>An account has been created for you at our service. Please use the following code to verify your email address:</p>
            <h1 style='letter-spacing: 5px; color: #4A90E2;'>{otpCode}</h1>
            <p>This code will expire in 15 minutes, and your password is: {password}.</p>
            <p>If this is not true, please ignore or report to that an account of your details was created using Admin account {currentUser.UserName} on this date: {dateOfGeneration}.</p>
        </div>";

                        await _emailService.SendEmailAsync(user.Email!, "Your Verification Code", message);
                    }
                    else if (user.CountryCode != null && user.PhoneNumber != null && user.Email == null)
                    {
                        DateTime dateOfGeneration = DateTime.UtcNow;
                        var otpGen = await _userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber!);
                        var message = $"Hi,\nAn account has been created for you at our service. If this is not true, please ignore or report to that an account of your details was created using Admin account {currentUser.UserName} on this date: {dateOfGeneration}\nOtherwise, if this is valid, your OTP code is: {otpGen}, your username is {user.UserName} and your password is: {password}";
                        await _smsService.SendSmsAsync($"{user.CountryCode}{user.PhoneNumber}", message);
                    }
                    else
                    {
                        throw new ClientError(400, "Failed to reach authenticatable mode of communication");
                    }
                }
                return;
            }
            else
                throw new ClientError(400, $"User registration failed: {result.Errors}");

        }
        private const string AllowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()_-+=[{]};:<>|./?";

        public static string GenerateSecurePassword(int length)
        {
            // GetString method handles cryptographic randomness and character selection securely.
            string password = RandomNumberGenerator.GetString(AllowedChars, length);
            return password;
        }
    }
}
