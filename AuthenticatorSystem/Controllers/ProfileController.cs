using BusinessLogicLayer.DTOs.Profile;
using BusinessLogicLayer.Infrastructure;
using BusinessLogicLayer.Infrastructure.Documentation;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticatorSystem.Controllers
{
    [Route("api/profile")]
    [Authorize]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService; // Inject this!

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpPost("create")]
        [Authorize(Policy = UserPolicies.MustBeActivated)]
        [EndpointDocumentation("create-profile")]
        public async Task<IActionResult> CreateProfile([FromForm] CreateProfileDto payload)
        {
            var result = await _profileService.CreateProfile(payload);
            return Ok(result);
        }

        [HttpGet]
        [Authorize(Policy = UserPolicies.MustHaveProfile)]
        [EndpointDocumentation("get-profile")]
        public async Task<IActionResult> GetProfile()
        {
            var result = await _profileService.GetProfile();
            return Ok(result);
        }

        [HttpPut("update")]
        [Authorize(Policy = UserPolicies.MustHaveProfile)]
        [Authorize(Policy = UserPolicies.MustBeActivated)]
        [EndpointDocumentation("update-profile")]
        public async Task<IActionResult> UpdateProfile(UpdateProfileDto payload)
        {
            await _profileService.UpdateProfile(payload);
            return NoContent();
        }

        [HttpPut("update-profile-picture")]
        [Consumes("multipart/form-data")]
        [Authorize(Policy = UserPolicies.MustHaveProfile)]
        [Authorize(Policy = UserPolicies.MustBeActivated)]
        [EndpointDocumentation("update-profile-picture")]
        public async Task<IActionResult> UpdateProfilePicture([FromForm] IFormFile? profilePicture)
        {
            await _profileService.UpdateProfilePicture(profilePicture);
            return NoContent();
        }

        [HttpPut("activate")]
        [Authorize(Policy = UserPolicies.MustHaveProfile)]
        [EndpointDocumentation("activate-profile")]
        public async Task<IActionResult> ActivateProfile([FromBody] DeactivateAccountDto payload)
        {
            var result = await _profileService.ActivateProfile(payload);
            return Ok(result);
        }

        [HttpPut("deactivate")]
        [Authorize(Policy = UserPolicies.MustHaveProfile)]
        [Authorize(Policy = UserPolicies.MustBeActivated)]
        [EndpointDocumentation("deactivate-profile")]
        public async Task<IActionResult> DeactivateProfile([FromBody] DeactivateAccountDto payload)
        {
            await _profileService.DeactivateProfile(payload);
            return NoContent();
        }

        [HttpPost("confirm-phone-number")]
        [Authorize(Policy = UserPolicies.MustHaveProfile)]
        [Authorize(Policy = UserPolicies.MustBeActivated)]
        public async Task<IActionResult> ConfirmPhoneNumber()
        {
            await _profileService.ConfirmUnconfirmedProfilePhoneNumber();
            return NoContent();
        }

        [HttpPost("confirm-email")]
        [Authorize(Policy = UserPolicies.MustHaveProfile)]
        [Authorize(Policy = UserPolicies.MustBeActivated)]
        public async Task<IActionResult> ConfirmEmail()
        {
            await _profileService.ConfirmUnconfirmedProfileEmail();
            return NoContent();
        }

        [HttpPut("update-phone-number")]
        [Authorize(Policy = UserPolicies.MustHaveProfile)]
        [Authorize(Policy = UserPolicies.MustBeActivated)]
        public async Task<IActionResult> UpdatePhoneNumber([FromBody] UpdatePhoneNumberDto payload)
        {
            await _profileService.UpdateProfilePhoneNumber(payload);
            return NoContent();
        }

        [HttpPut("update-email")]
        [Authorize(Policy = UserPolicies.MustHaveProfile)]
        [Authorize(Policy = UserPolicies.MustBeActivated)]
        public async Task<IActionResult> UpdateEmail([FromBody] UpdateEmailDto payload)
        {
            await _profileService.UpdateProfileEmail(payload);
            return NoContent();
        }

        [HttpPut("change-password")]
        [Authorize(Policy = UserPolicies.MustHaveProfile)]
        [Authorize(Policy = UserPolicies.MustBeActivated)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto payload)
        {
            await _profileService.ChangePassword(payload);
            return NoContent();
        }

        [HttpPut("set-preferred-contact-mode")]
        [Authorize(Policy = UserPolicies.MustHaveProfile)]
        [Authorize(Policy = UserPolicies.MustBeActivated)]
        [EndpointDocumentation("deactivate-profile")]
        public async Task<IActionResult> SetPreferredContactMode([FromBody] SetPreferredModeDto payload)
        {
            await _profileService.SetPreferredContactMode(payload.Mode);
            return NoContent();
        }
    }
}
