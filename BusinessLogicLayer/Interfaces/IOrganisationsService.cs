using BusinessLogicLayer.DTOs.Authentication;
using BusinessLogicLayer.DTOs.Organisations;
using BusinessLogicLayer.DTOs.Profile;
using BusinessLogicLayer.DTOs.ProfileOrganisation;
using DataAccessLayer.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace BusinessLogicLayer.Interfaces
{
    public interface IOrganisationService
    {
        Task<AuthResultDto> CreateOrganisation(CreateOrganisationDto payload);
        Task InviteUserToDomain(string profileId);
        Task<AuthResultDto> AcceptInvitation(string organisationId, bool invitationAccepted);
        Task<AuthResultDto> SwitchOrganisation(string organisationId);
        Task<GetOrganisationDto> GetOrganisation(string organisationId);
        Task<GetOrganisationDto> GetCurrentOrganisation();
        Task<List<GetOrganisationsDto>> GetOrganisations();
        Task<List<GetOrganisationsDto>> GetPublicOrganisations();
        Task<List<GetMyOrganisationDto>> GetMyOrganisations();
        Task<List<GetProfileOrganisationDto>> GetOrganisationMembers(string organisationId);
        Task<OrganisationStatus> ChangeOrganisationStatus(string organisationId);
        Task UpdateOrganisation(UpdateProfileDto payload);
        Task UpdateOrganisationLogo(IFormFile? picture);
        Task UpdateOrganisationHeader(IFormFile? picture);
    }
}
