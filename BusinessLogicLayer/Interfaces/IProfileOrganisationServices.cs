using BusinessLogicLayer.DTOs.ProfileOrganisation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
    public interface IProfileOrganisationServices
    {
        Task CreateProfileOrganisation(CreateProfileOrganisationDto payload);
        Task<List<GetProfileOrganisationDto>> GetProfileOrganisationsByProfile(string profileid);
        Task<List<GetProfileOrganisationDto>> GetProfileOrganisationsByOrganisation(string organisationId);
        Task<GetProfileOrganisationDto> GetProfileOrganisation(string profileid, string organisationId);
    }
}
