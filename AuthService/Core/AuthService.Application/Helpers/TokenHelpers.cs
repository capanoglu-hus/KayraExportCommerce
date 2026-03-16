using AuthService.Application.Dtos;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.Application.Helpers
{
    public class TokenHelpers
    {
        private readonly IConfiguration _configuration;

        public TokenHelpers(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(TokenClaimsDto tokenDto)
        {
            /*token oluşturma*/
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creadentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // burada kullanıcı bilgileri alacaz
            var claims = new List<Claim>
            {
                new Claim("_e" ,tokenDto.Email),
                new Claim("_u" ,tokenDto.Id),
                new Claim("_r",tokenDto.Role),
                new Claim(JwtRegisteredClaimNames.Jti,  Guid.NewGuid().ToString()),
            };
            var token = new JwtSecurityToken
                (
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creadentials
                );
            var resultToken = new JwtSecurityTokenHandler().WriteToken(token);
            return resultToken;
            
        }
    }
}
