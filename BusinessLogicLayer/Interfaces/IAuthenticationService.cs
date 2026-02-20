using BusinessLogicLayer.DTOs.Authentication;
using BusinessLogicLayer.Infrastructure;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
    public interface IAuthenticationService
    {
        CheckApiDto CheckApi();
        CheckApiDto CheckAuth();
        Task ResendEmailOtpAsync();
        Task ResendSmsOtpAsync();
        Task<AuthResultDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthResultDto> ConfirmEmailWithCodeAsync(NewUserCodeDto newUserCode);
        Task<AuthResultDto> ConfirmPhoneNumber(NewUserCodeDto newUserCode);
        Task<AuthResultDto> LoginAsync(LoginDto loginDto);
        Task<AuthResultDto> RefreshTokenAsync(TokenRequestDto tokenRequestDto);
        Task RequestForgotPassword(string username);
        Task ConfirmForgotPassword(ResetPasswordDto resetPassword);
        Task LogoutAsync();
        //Task<AuthResultDto> GenerateJwtToken(ApplicationUser user, bool stayLoggedIn, string existingRefreshToken);
    }
}
