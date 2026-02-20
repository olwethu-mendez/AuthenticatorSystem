using BusinessLogicLayer.DTOs.User;
using BusinessLogicLayer.Infrastructure;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticatorSystem.Controllers
{
    [Route("api/users")]
    [ApiController]
    [Authorize(Policy = UserPolicies.MustBeActivated)]
    [Authorize(Policy = UserPolicies.MustHaveProfile)]
    [Authorize(Roles = UserRoles.Admin)]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;
        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUser(CreateUserDto payload)
        {
            await _usersService.CreateUser(payload);
            return Created();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _usersService.GetAllUsers();
            return Ok(result);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetSingleUser(string userId)
        {
            var result = await _usersService.GetSingleUser(userId);
            return Ok(result);
        }

        [HttpPut("deactivate/{userId}")]
        public async Task<IActionResult> AdminDeactivatesUser(string userId)
        {
            var result = await _usersService.AdminDeactivatesUser(userId);
            return Ok(result);
        }
    }
}
