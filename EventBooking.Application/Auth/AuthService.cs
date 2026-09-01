using EventBooking.Application.Auth.DTOs;
using EventBooking.Application.Common.Exceptions;
using EventBooking.Application.Common.Interfaces;
using EventBooking.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace EventBooking.Application.Auth
{
    public class AuthService(UserManager<ApplicationUser> _userManager, ITokenService _tokenService) : IAuthService
    {
        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
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
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new ValidationException(errors);
            }

            await _userManager.AddToRoleAsync(user, "Attendee");

            return await BuildAuthResponseAsync(user);
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user is null)
            {
                throw new ValidationException("البريد الإلكتروني أو كلمة المرور غير صحيحة");
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);

            if (!isPasswordValid)
            {
                throw new ValidationException("البريد الإلكتروني أو كلمة المرور غير صحيحة");
            }

            return await BuildAuthResponseAsync(user);
        }

        private async Task<AuthResponse> BuildAuthResponseAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.GenerateToken(user, roles);

            return new AuthResponse
            {
                Token = token,
                Email = user.Email!,
                FullName = user.FullName,
                Roles = roles
            };
        }
    }
}