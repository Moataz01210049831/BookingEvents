using Azure.Core;
using EventBooking.Application.Auth.DTOs;
using EventBooking.Application.Common.Interfaces;
using EventBooking.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EventBooking.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController (UserManager<ApplicationUser> _userManager,ITokenService _tokenService): ControllerBase
    {

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody]RegisterRequest request)
        {
            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FullName = request.FullName,
                CreatedAtUtc = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.Select(e => e.Description));
            }

            await _userManager.AddToRoleAsync(user, "Attendee");
            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.GenerateToken(user, roles);

            return Ok(new AuthResponse
            {
                Token = token,
                Email = user.Email!,
                FullName = user.FullName
            });
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
            {
                return Unauthorized("Invalid email or password");
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isPasswordValid)
            {
                return Unauthorized("Invalid email or password");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.GenerateToken(user, roles);

            return Ok(new AuthResponse
            {
                Token = token,
                Email = user.Email!,
                FullName = user.FullName
            });
        }


    }
}
