using BusinessLogicLayer.DTOs.Authentication;
using BusinessLogicLayer.DTOs.Organisations;
using BusinessLogicLayer.Infrastructure;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticatorSystem.Controllers
{
    [Route("api/organisation")]
    [Authorize]
    [ApiController]
    public class OrganisationController : ControllerBase
    {
        private readonly IOrganisationService _organisationService;
        public OrganisationController(IOrganisationService organisationService)
        {
            _organisationService = organisationService;
        }

        [HttpPost("create")]
        [Authorize(Policy = UserPolicies.MustHaveProfile)]
        [Authorize(Policy = UserPolicies.MustBeActivated)]
        public async Task<IActionResult> CreateOrganisation([FromForm] CreateOrganisationDto payload)
        {
            var result = await _organisationService.CreateOrganisation(payload);
            return Ok(result);
        }

        [HttpPost("invitation")]
        [Authorize(Roles = UserRoles.OrgAdmin)]
        [Authorize(Policy = UserPolicies.MustHaveProfile)]
        [Authorize(Policy = UserPolicies.MustBeActivated)]
        public async Task<IActionResult> InviteUserToDomain([FromQuery] string profileId)
        {
            await _organisationService.InviteUserToDomain(profileId);
            return Ok();
        }

        [HttpPut("accept-invitation")]
        [Authorize(Policy = UserPolicies.MustHaveProfile)]
        [Authorize(Policy = UserPolicies.MustBeActivated)]
        public async Task<IActionResult> AcceptInvitation([FromQuery] string organisationId, bool invitationAccepted)
        {
            var result = await _organisationService.AcceptInvitation(organisationId, invitationAccepted);
            return Ok(result);
        }

        [HttpGet("get-organisation/{organisationId}")]
        [Authorize(Policy = UserPolicies.MustHaveProfile)]
        [Authorize(Policy = UserPolicies.MustBeActivated)]
        public async Task<IActionResult> GetOrganisation(string organisationId)
        {
            var result = await _organisationService.GetOrganisation(organisationId);
            return Ok(result);
        }

        [HttpGet("get-organisations")]
        [Authorize(Policy = UserPolicies.MustHaveProfile)]
        [Authorize(Policy = UserPolicies.MustBeActivated)]
        public async Task<IActionResult> GetOrganisations()
        {
            var result = await _organisationService.GetOrganisations();
            return Ok(result);
        }

        [HttpGet("my-organisations")]
        [Authorize(Policy = UserPolicies.MustHaveProfile)]
        [Authorize(Policy = UserPolicies.MustBeActivated)]
        public async Task<IActionResult> GetMyOrganisations()
        {
            var result = await _organisationService.GetMyOrganisations();
            return Ok(result);
        }

        [HttpGet("public-organisations")]
        [Authorize(Policy = UserPolicies.MustHaveProfile)]
        [Authorize(Policy = UserPolicies.MustBeActivated)]
        public async Task<IActionResult> GetPublicOrganisations()
        {
            var result = await _organisationService.GetPublicOrganisations();
            return Ok(result);
        }

        [HttpPost("switch-organisation/{organisationId}")]
        public async Task<IActionResult> SwitchOrganisation(string organisationId)
        {
            var token = await _organisationService.SwitchOrganisation(organisationId);
            return Ok(token);
        }
    }
}
