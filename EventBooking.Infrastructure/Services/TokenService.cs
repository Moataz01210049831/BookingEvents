using EventBooking.Application.Common.Interfaces;
using EventBooking.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EventBooking.Infrastructure.Services
{
    public class TokenService(IConfiguration _configuration) : ITokenService

    {
       

        public string GenerateToken(ApplicationUser user, IList<string> roles)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["Secret"]!;
            var claims = new List<Claim> {
                 new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            foreach (var role in roles)
            {
                claims.Add(new Claim("roles", role));
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiryMinutes = double.Parse(jwtSettings["ExpiryMinutes"]!);

            var token = new JwtSecurityToken(
              issuer: jwtSettings["Issuer"],
              audience: jwtSettings["Audience"],
              claims: claims,
              expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
              signingCredentials: credentials

          );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}
