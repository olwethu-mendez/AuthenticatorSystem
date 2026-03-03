using BusinessLogicLayer.DTOs.Authentication;
using BusinessLogicLayer.DTOs.Organisations;
using BusinessLogicLayer.Interfaces;
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

        [HttpPost()]
        public async Task<IActionResult> CreateOrganisation([FromForm] CreateOrganisationDto payload)
        {
            return Ok();
        }
        public async Task<IActionResult> InviteUserToDomain(string profileId)
        {
            return Ok();
        }
        public async Task<IActionResult> AcceptInvitation(string organisationId, bool invitationAccepted)
        {
            return Ok();
        }
        public async Task<IActionResult> GetOrganisation(string organisationId)
        {
            return Ok();
        }
        public async Task<IActionResult> GetMyOrganisations()
        {
            return Ok();
        }
    }
}
