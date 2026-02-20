using BusinessLogicLayer.DTOs.Profile;
using BusinessLogicLayer.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
    public interface IUsersService
    {
        Task<GetUserDto?> GetSingleUser(string profileId);
        Task<List<GetUsersListDto>> GetAllUsers();
        Task<string> AdminDeactivatesUser(string accountUserId);
        Task CreateUser(CreateUserDto payload);
    }
}
