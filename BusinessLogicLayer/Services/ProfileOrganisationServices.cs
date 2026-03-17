using BusinessLogicLayer.DTOs.ProfileOrganisation;
using BusinessLogicLayer.Infrastructure;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Data;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class ProfileOrganisationServices : IProfileOrganisationServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IUsersService _usersService;
        private readonly IOrganisationService _organisationService;
        public ProfileOrganisationServices(ApplicationDbContext context, IUsersService usersService, IOrganisationService organisationService)
        {
            _context = context;
            _usersService = usersService;
            _organisationService = organisationService;
        }
        public async Task CreateProfileOrganisation(CreateProfileOrganisationDto payload)
        {
            var profileOrganisation = new ProfileOrganisation
            {
                ProfileId = payload.ProfileId,
                OrganisationId = payload.OrganisationId,
                InvitationAccepted = payload.InvitationAccepted,
                IsOrgAdmin = payload.IsOrgAdmin
            };
            await _context.ProfileOrganisations.AddAsync(profileOrganisation);
            await _context.SaveChangesAsync();
        }

        public async Task<GetProfileOrganisationDto> GetProfileOrganisation(string profileid, string organisationId)
        {
            var profileOrganisation = await _context.ProfileOrganisations
                .FirstOrDefaultAsync(x =>
                    x.ProfileId == profileid &&
                    x.OrganisationId == organisationId);

            if (profileOrganisation == null)
                throw new ClientError(404, "Profile organisation not found.");

            var profile = await _usersService.GetUserById(profileid);
            var organisation = await _organisationService.GetOrganisation(organisationId);

            return new GetProfileOrganisationDto
            {
                Profile = profile,
                Organisation = organisation,
                InvitationAccepted = profileOrganisation.InvitationAccepted,
                IsOrgAdmin = profileOrganisation.IsOrgAdmin ?? false
            };
        }

        public async Task<List<GetProfileOrganisationDto>> GetProfileOrganisationsByOrganisation(string organisationId)
        {
            var profileOrganisations = await _context.ProfileOrganisations
                .Where(x => x.OrganisationId == organisationId)
                .ToListAsync();

            var organisation = await _organisationService.GetOrganisation(organisationId);

            var result = new List<GetProfileOrganisationDto>();

            foreach (var item in profileOrganisations)
            {
                var profile = await _usersService.GetUserById(item.ProfileId);

                result.Add(new GetProfileOrganisationDto
                {
                    Profile = profile,
                    Organisation = organisation,
                    InvitationAccepted = item.InvitationAccepted,
                    IsOrgAdmin = item.IsOrgAdmin ?? false
                });
            }

            return result;
        }

        public async Task<List<GetProfileOrganisationDto>> GetProfileOrganisationsByProfile(string profileid)
        {
            var profileOrganisations = await _context.ProfileOrganisations
                .Where(x => x.ProfileId == profileid)
                .ToListAsync();

            var profile = await _usersService.GetUserById(profileid);

            var result = new List<GetProfileOrganisationDto>();

            foreach (var item in profileOrganisations)
            {
                var organisation = await _organisationService.GetOrganisation(item.OrganisationId);

                result.Add(new GetProfileOrganisationDto
                {
                    Profile = profile,
                    Organisation = organisation,
                    InvitationAccepted = item.InvitationAccepted,
                    IsOrgAdmin = item.IsOrgAdmin ?? false
                });
            }

            return result;
        }
    }
}
