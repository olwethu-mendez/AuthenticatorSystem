using BusinessLogicLayer.DTOs.Authentication;
using BusinessLogicLayer.Infrastructure.Documentation;
using BusinessLogicLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthenticatorSystem.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authService;

        public AuthenticationController(IAuthenticationService authService)
        {
            _authService = authService;
        }


        [HttpGet("check")]
        public IActionResult CheckApi()
        {
            var result = _authService.CheckApi();
            return Ok(result);
        }

        [HttpGet("check-auth")]
        [Authorize]
        public IActionResult CheckAuth()
        {
            var result = _authService.CheckAuth();
            return Ok(result);
        }

        [HttpPost("register")]
        [EndpointDocumentation("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto payload)
        {
            var result = await _authService.RegisterAsync(payload);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPost("confirm-phone")]
        [Authorize]
        public async Task<IActionResult> ConfirmPhone([FromBody] NewUserCodeDto payload)
        {
            var results = await _authService.ConfirmPhoneNumber(payload);
            return Ok(results);
        }

        [HttpPost("confirm-email")]
        [Authorize]
        public async Task<IActionResult> ConfirmEmail([FromBody] NewUserCodeDto payload)
        {
            var results = await _authService.ConfirmEmailWithCodeAsync(payload);
            return Ok(results);
        }

        [HttpPost("resend-otp/{type}")]
        [Authorize]
        public async Task<IActionResult> ResendOtp(string type)
        {
            if (type == "email") await _authService.ResendEmailOtpAsync();
            else if (type == "sms") await _authService.ResendSmsOtpAsync();
            else BadRequest(new { error = "Invalid otp type. Must either be email or sms" });
            return NoContent();
        }

        [HttpPost("login")]
        [EndpointDocumentation("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto payload)
        {
            var result = await _authService.LoginAsync(payload);
            return Ok(result);
        }

        [HttpPost("forgot-password")]
        [EndpointDocumentation("forgot-password")]
        public async Task<IActionResult> RequestForgotPassword(string username)
        {
            await _authService.RequestForgotPassword(username);
            return Ok();
        }

        [HttpPost("confirm-forgot-password")]
        [EndpointDocumentation("confirm-forgot-password")]
        public async Task<IActionResult> ConfirmForgotPassword(ResetPasswordDto resetPassword)
        {
            await _authService.ConfirmForgotPassword(resetPassword);
            return Ok();
        }

        [HttpPost("refresh-token")]
        [EndpointDocumentation("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequestDto payload)
        {
            var result = await _authService.RefreshTokenAsync(payload);
            return Ok(result);
        }

        [HttpPost("logout")]
        [EndpointDocumentation("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            return NoContent();
        }
    }
}
