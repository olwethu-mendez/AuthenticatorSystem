using BusinessLogicLayer.DTOs.Authentication;
using BusinessLogicLayer.DTOs.Organisations;
using BusinessLogicLayer.DTOs.Profile;
using Microsoft.AspNetCore.Http;

namespace BusinessLogicLayer.Interfaces
{
    public interface IOrganisationService
    {
        Task<AuthResultDto> CreateOrganisation(CreateOrganisationDto payload);
        Task InviteUserToDomain(string profileId, string organisationId);
        Task AcceptInvitation(string organisationId, bool invitationAccepted);
        Task<GetOrganisationDto> GetOrganisation(string organisationId);
        //Task UpdateProfile(UpdateProfileDto payload);
        //Task UpdateProfilePicture(IFormFile? profilePicture);
        //Task<AuthResultDto> ActivateProfile(DeactivateAccountDto payload);
        //Task DeactivateProfile(DeactivateAccountDto payload);
    }
}
