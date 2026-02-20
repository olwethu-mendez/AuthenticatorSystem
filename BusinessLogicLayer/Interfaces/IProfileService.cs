using BusinessLogicLayer.DTOs.Authentication;
using BusinessLogicLayer.DTOs.Profile;
using BusinessLogicLayer.Services;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
    public interface IProfileService
    {
        Task<AuthResultDto> CreateProfile(CreateProfileDto payload);
        Task<GetProfileDto> GetProfile();
        Task UpdateProfile(UpdateProfileDto payload);
        Task UpdateProfilePicture(IFormFile? profilePicture);
        Task<AuthResultDto> ActivateProfile(DeactivateAccountDto payload);
        Task DeactivateProfile(DeactivateAccountDto payload);

        Task UpdateProfileEmail(UpdateEmailDto payload);
        Task UpdateProfilePhoneNumber(UpdatePhoneNumberDto payload);

        Task ConfirmUnconfirmedProfileEmail();
        Task ConfirmUnconfirmedProfilePhoneNumber();
        Task SetPreferredContactMode(string mode);
        Task ChangePassword(ChangePasswordDto payload);


        Task<Dictionary<string, string>> InitialProfilePictureUpload(IFormFile formFile);
        Task<Dictionary<string, string>> UploadProfilePicture(Profile profile, IFormFile formFile);
    }
}
