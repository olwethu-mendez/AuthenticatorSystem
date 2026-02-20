using BusinessLogicLayer.DTOs.Authentication;
using BusinessLogicLayer.Infrastructure;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Data;
using DataAccessLayer.Infrastructure;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Mysqlx.Session;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Twilio;

namespace BusinessLogicLayer.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly TokenService _tokenService;
        private readonly ContextAccessorService _contextAccessor;
        private readonly ISmsService _smsService;
        private readonly IEmailService _emailService;

        public AuthenticationService(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            IConfiguration configuration,
            TokenValidationParameters tokenValidationParameters,
            TokenService tokenService,
            ContextAccessorService contextAccessor,
            ISmsService smsService,
            IEmailService emailService)
        {
            _userManager = userManager;
            _context = context;
            _configuration = configuration;
            _tokenValidationParameters = tokenValidationParameters;
            _tokenService = tokenService;
            _contextAccessor = contextAccessor;
            _smsService = smsService;
            _emailService = emailService;
        }

        public CheckApiDto CheckApi()
        {
            return new CheckApiDto
            {
                Status = ApiStatus.isOnline,
                Username = null,
            };
        }

        public CheckApiDto CheckAuth()
        {
            var userId = _contextAccessor.GetCurrentUserId();
            if (userId == null) throw new ClientError(401, "User unauthenticated");
            var user = _userManager.Users.Where(u => u.Id == userId).FirstOrDefault();
            if (user == null) throw new ClientError(404, "Can't check if Api is auth. User not found");
            return new CheckApiDto
            {
                Status = ApiStatus.isAuthenticated,
                Username = user.UserName,
            };
        }

        public async Task<AuthResultDto> LoginAsync(LoginDto loginDto)
        {
            string phoneNumberRegex = @"^\d{9}$";
            string countryCodeRegex = @"^\+\d{1,3}$";
            string emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            ApplicationUser? user = null;

            if (loginDto.CountryCode != null && Regex.IsMatch(loginDto.CountryCode, countryCodeRegex) && Regex.IsMatch(loginDto.Username, phoneNumberRegex))
            {
                user = await _userManager.Users.FirstOrDefaultAsync(u => u.CountryCode == loginDto.CountryCode && u.PhoneNumber == loginDto.Username);
                if (user == null)
                {
                    throw new ClientError(401, "Invalid username or password.");
                }
                if(user.PhoneNumberConfirmed == false && user.EmailConfirmed == true)
                {
                    throw new ClientError(401, "Login method unconfirmed.");
                }
            }

            if (Regex.IsMatch(loginDto.Username, emailRegex))
            {
                user = await _userManager.FindByEmailAsync(loginDto.Username);
                if (user == null)
                {
                    throw new ClientError(401, "Invalid username or password.");
                }
                if (user.EmailConfirmed == false && user.PhoneNumberConfirmed == true)
                {
                    throw new ClientError(401, "Login method unconfirmed.");
                }
            }

            if (user == null) throw new ClientError(400, "Invalid login credentials format.");

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!isPasswordValid)
            {
                throw new ClientError(401, "Invalid username or password.");
            }
            if (await _userManager.IsLockedOutAsync(user))
            {
                throw new ClientError(403, "User account is locked.");
            }
            var tokenResponse = await _tokenService.GenerateJwtToken(user, loginDto.StayLoggedIn ?? false, null);
            return tokenResponse;
        }

        public async Task RequestForgotPassword(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return; /*{
                var userByEmail = await _context.ApplicationUsers.Where(x => x.Email == username && x.EmailConfirmed).FirstOrDefaultAsync();
                var userByPhone = await _context.ApplicationUsers.Where(x => x.PhoneNumber == username && x.PhoneNumberConfirmed).FirstOrDefaultAsync();
                if (userByEmail != null) user = userByEmail;
                else if (userByPhone != null) user = userByPhone;
                else return;
            }*/
            var code = await _userManager.GeneratePasswordResetTokenAsync(user!);

            if(user.UserName == user.PhoneNumber){
                var message = $"Hi,\nSeems you forgot your password.\nYour code is: {code}\nIf you didn't request this, you can safely ignore this message.";
                await _smsService.SendSmsAsync($"{user.CountryCode}{user.PhoneNumber}", message);
            }
            else if(user.UserName == user.Email){
                var message = $@"
        <div style='font-family: Arial, sans-serif;'>
            <h2>Verify your email</h2>
            <p>It seems you've forgotten your password. Please use the following code to verify your account and be abe to update your password:</p>
            <h1 style='letter-spacing: 5px; color: #4A90E2;'>{code}</h1>
            <p>This code will expire in 15 minutes.</p>
        </div>";

                await _emailService.SendEmailAsync(user.Email!, "Forgot Password Code", message);

            }
        }

        public async Task ConfirmForgotPassword(ResetPasswordDto resetPassword)
        {
            var user = await _userManager.FindByNameAsync(resetPassword.Username);
            if (user == null) throw new ClientError(400, "Failed to reset password. User not found.");
            var oldPassword = _context.PreviousPasswords.Where(x => x.UserId == user.Id).Select(x => x.PasswordHash).ToList();

            if(oldPassword.Any(oldHash => _userManager.PasswordHasher.VerifyHashedPassword(user, oldHash, resetPassword.NewPassword) == PasswordVerificationResult.Success))
            {
                throw new ClientError(400, "You cannot use a previously used password. Please choose a different one.");
            }
            if (!string.IsNullOrEmpty(user.PasswordHash))
            {
                var previousPassword = new PreviousPasswords
                {
                    UserId = user.Id,
                    PasswordHash = user.PasswordHash,
                    DateSet = DateTime.UtcNow,
                };
                _context.PreviousPasswords.Add(previousPassword);
                await _context.SaveChangesAsync();
            }
            var result = await _userManager.ResetPasswordAsync(user, resetPassword.Code, resetPassword.NewPassword);
            if (!result.Succeeded)
            {
                throw new ClientError(400, "Password not reset");
            }
        }

        public async Task<AuthResultDto> ConfirmEmailWithCodeAsync(NewUserCodeDto newUserCode)
        {
            var loggedInUser = _contextAccessor.GetCurrentUserId();
            if (loggedInUser != newUserCode.userId) throw new ClientError(403, "Confirming email failed. Authenticated user does not match user being verified");
            var user = await _userManager.FindByIdAsync(newUserCode.userId);
            if (user == null) throw new ClientError(404, "User not found");

            if (user.EmailConfirmed == true) throw new ClientError(400, "Email already confirmed");

            // Verify the 6-digit code
            var isValid = await _userManager.VerifyTwoFactorTokenAsync(user, "Email", newUserCode.code);

            if (!isValid)
            {
                throw new ClientError(400, "Invalid or expired verification code.");
            }

            // Manually mark email as confirmed since VerifyTwoFactorTokenAsync 
            // only checks the code but doesn't update the user record.
            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);

            await LogoutAsync();
            var newToken = await _tokenService.GenerateJwtToken(user, false, null);
            return newToken;
        }

        public async Task<AuthResultDto> ConfirmPhoneNumber(NewUserCodeDto newUserCode)
        {
            var loggedInUser = _contextAccessor.GetCurrentUserId();
            if (loggedInUser != newUserCode.userId) throw new ClientError(403, "Confirming email failed. Authenticated user does not match user being verified");
            var user = await _userManager.FindByIdAsync(newUserCode.userId);
            if (user == null) throw new ClientError(404, "Failed to Confirm phone number. User not found");

            if (user.PhoneNumberConfirmed == true) throw new ClientError(400, "Phone number already confirmed");

            var result = await _userManager.ChangePhoneNumberAsync(user, user.PhoneNumber!, newUserCode.code);
            if (!result.Succeeded) throw new ClientError(400, "Invalid or expired verification code");

            await LogoutAsync();
            var newToken = await _tokenService.GenerateJwtToken(user, false, null);
            return newToken;
        }

        public async Task ResendSmsOtpAsync()
        {
            var userId = _contextAccessor.GetCurrentUserId();
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new ClientError(404, "User not found");

            if (user.PhoneNumberConfirmed)
                throw new ClientError(400, "User phone number is already verified");

            await SendOtpSmsAsync(user);
        }

        public async Task ResendEmailOtpAsync()
        {
            var userId = _contextAccessor.GetCurrentUserId();
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new ClientError(404, "User not found");

            if (user.EmailConfirmed)
                throw new ClientError(400, "User phone number is already verified");

            await SendEmailOtpAsync(user);
        }

        public async Task LogoutAsync()
        {
            var userId = _contextAccessor.GetCurrentUserId();
            var refreshTokens = await _context.RefreshTokens
                .Where(x => x.UserId == userId && !x.IsRevoked).ToListAsync();

            foreach (var token in refreshTokens) token.IsRevoked = true;

            var accessToken = _contextAccessor.GetRequestHeaders("Authorization")?.Split(" ").Last();
            if (!string.IsNullOrEmpty(accessToken))
            {
                await _tokenService.BlacklistToken(accessToken);
            }
            await _context.SaveChangesAsync();
        }

        public async Task<AuthResultDto> RefreshTokenAsync(TokenRequestDto tokenRequestDto)
        {
            try
            {
                var results = await GetPrincipalFromExpiredToken(tokenRequestDto);
                if (results == null)
                {
                    throw new ClientError(400, "Invalid token.");
                }
                return results;
            }
            catch (Exception ex)
            {
                throw new ClientError(400, $"Invalid token or issue found: {ex.Message}.");
            }
        }

        public async Task<AuthResultDto> RegisterAsync(RegisterDto registerDto)
        {
            if (string.IsNullOrEmpty(registerDto.Email) && string.IsNullOrEmpty(registerDto.PhoneNumber))
                throw new ClientError(400, "at least an email or phone number must be provided");
            if (!string.IsNullOrEmpty(registerDto.Email) && !string.IsNullOrEmpty(registerDto.PhoneNumber) && registerDto.PrefersEmail == null)
                throw new ClientError(400, "If both phone and email are provided user must select their preferred mode of communication");

            if (!string.IsNullOrEmpty(registerDto.Email))
            {
                var userEmailExists = await _userManager.FindByEmailAsync(registerDto.Email);
                if (userEmailExists != null)
                {
                    throw new ClientError(400, "Email is already registered.");
                }
            }
            if (!string.IsNullOrEmpty(registerDto.PhoneNumber))
            {
                var userPhoneExists = await _userManager.Users.FirstOrDefaultAsync(u => u.CountryCode == registerDto.CountryCode && u.PhoneNumber == registerDto.PhoneNumber);
                if (userPhoneExists != null)
                {
                    throw new ClientError(400, "Phone number is already registered.");
                }
            }

            string preferredCommunication;
            if (!string.IsNullOrEmpty(registerDto.Email?.Trim()) && !string.IsNullOrEmpty(registerDto.PhoneNumber?.Trim()))
            {
                if (registerDto.PrefersEmail == true) preferredCommunication = registerDto.Email;
                else if (registerDto.PrefersEmail == false) preferredCommunication = registerDto.PhoneNumber;
                else preferredCommunication = registerDto.Email;
            }
            else if (string.IsNullOrEmpty(registerDto.Email) && !string.IsNullOrEmpty(registerDto.PhoneNumber))
            {
                preferredCommunication = registerDto.PhoneNumber;
            }
            else
            {
                preferredCommunication = registerDto.Email!;
            }

            ApplicationUser user = new ApplicationUser()
            {
                UserName = preferredCommunication,
                Email = string.IsNullOrWhiteSpace(registerDto.Email) ? null : registerDto.Email,
                PhoneNumber = string.IsNullOrWhiteSpace(registerDto.PhoneNumber) ? null : registerDto.PhoneNumber,
                CountryCode = string.IsNullOrWhiteSpace(registerDto.CountryCode) ? null : registerDto.CountryCode,
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
            };
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (result.Succeeded)
            {
                var previousPassword = new PreviousPasswords()
                {
                    UserId = user.Id,
                    PasswordHash = user.PasswordHash ?? throw new ClientError(400, "Invalid password"),
                    DateSet = DateTime.UtcNow
                };
                await _userManager.AddToRoleAsync(user, UserRoles.General);
                await _context.PreviousPasswords.AddAsync(previousPassword);
                await _context.SaveChangesAsync();
                if (registerDto.PrefersEmail == true)
                {
                    await SendEmailOtpAsync(user);
                }
                else if (registerDto.PrefersEmail == false)
                {
                    await SendOtpSmsAsync(user);
                }
                else
                {
                    if (user.Email != null && user.PhoneNumber == null)
                    {
                        await SendEmailOtpAsync(user);
                    }
                    else if (user.CountryCode != null && user.PhoneNumber != null && user.Email == null)
                    {
                        await SendOtpSmsAsync(user);
                    }
                    else
                    {
                        throw new ClientError(400, "Failed to reach authenticatable mode of communication");
                    }
                }


                var tokenResponse = await _tokenService.GenerateJwtToken(user, false, null);
                return tokenResponse;
            }
            throw new ClientError(400, $"User registration failed");
        }

        private async Task<AuthResultDto?> GetPrincipalFromExpiredToken(TokenRequestDto payload)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var validationParameters = _tokenValidationParameters.Clone();
            validationParameters.ValidateLifetime = false; // allow expired access token

            var principal = tokenHandler.ValidateToken(
                payload.Token,
                validationParameters,
                out var validatedToken
            );

            if (validatedToken is not JwtSecurityToken jwt ||
                !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ClientError(400, "Invalid access token");
            }

            var expUnix = long.Parse(
                principal.Claims.First(x => x.Type == JwtRegisteredClaimNames.Exp).Value
            );

            var tokenExpired = DateTimeOffset
                .FromUnixTimeSeconds(expUnix)
                .UtcDateTime <= DateTime.UtcNow;

            var isBlacklisted = await _context.TokenBlacklists
                .AnyAsync(x => x.Token == payload.Token);

            // If token is still valid and not blacklisted → do NOT refresh
            if (!tokenExpired && !isBlacklisted)
            {
                throw new ClientError(400, "Token still valid, refresh not required");
            }

            // Validate refresh token
            var dbRefreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(x => x.Token == payload.RefreshToken);

            if (dbRefreshToken == null)
                throw new ClientError(400, "Refresh token does not exist");

            if (dbRefreshToken.DateExpires <= DateTime.UtcNow)
                throw new ClientError(400, "Refresh token expired");

            if (dbRefreshToken.IsRevoked)
                throw new ClientError(400, "Refresh token revoked");

            var jti = principal.Claims
                .First(x => x.Type == JwtRegisteredClaimNames.Jti)
                .Value;

            if (dbRefreshToken.JwtId != jti)
                throw new ClientError(400, "Refresh token does not match access token");

            var user = await _userManager.FindByIdAsync(dbRefreshToken.UserId);
            if (user == null)
                throw new ClientError(400, "User not found");

            // Generate new JWT
            var authResult = await _tokenService.GenerateJwtToken(
                user,
                dbRefreshToken.StayLoggedIn,
                dbRefreshToken
            );

            return authResult;
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
            var otpCode = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");

            var message = $@"
        <div style='font-family: Arial, sans-serif;'>
            <h2>Verify your email</h2>
            <p>Thank you for registering. Please use the following code to verify your email address:</p>
            <h1 style='letter-spacing: 5px; color: #4A90E2;'>{otpCode}</h1>
            <p>This code will expire in 15 minutes.</p>
        </div>";

            await _emailService.SendEmailAsync(user.Email!, "Your Verification Code", message);
        }
    }
}
