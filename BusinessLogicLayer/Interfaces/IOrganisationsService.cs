using BusinessLogicLayer.DTOs.Authentication;
using BusinessLogicLayer.DTOs.Organisations;
using BusinessLogicLayer.DTOs.Profile;
using Microsoft.AspNetCore.Http;

namespace BusinessLogicLayer.Interfaces
{
    public interface IOrganisationService
    {
        Task<AuthResultDto> CreateOrganisation(CreateOrganisationDto payload);
        Task InviteUserToDomain(string profileId);
        Task<AuthResultDto> AcceptInvitation(string organisationId, bool invitationAccepted);
        Task<GetOrganisationDto> GetOrganisation(string organisationId);
        Task<List<GetOrganisationsDto>> GetOrganisations();
        Task<List<GetOrganisationsDto>> GetPublicOrganisations();
        Task<List<GetMyOrganisationDto>> GetMyOrganisations();
        //Task UpdateProfile(UpdateProfileDto payload);
        //Task UpdateProfilePicture(IFormFile? profilePicture);
        //Task<AuthResultDto> ActivateProfile(DeactivateAccountDto payload);
        //Task DeactivateProfile(DeactivateAccountDto payload);
    }
}
