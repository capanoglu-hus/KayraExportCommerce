using AuthApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuthApi.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreateAccessToken(User user, IList<string> roles)
        {
            var jwt = _configuration.GetSection("Jwt"); //appsettings 
            // secret key byte çevirip , simetrik güvenlik anahtarı oluşturur
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["SecurityKey"]!));
            //HmacSha256 ile imzalama 
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            /* token içinde bulunan özellikler*/
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),

            };
            // token sürecine role ekleme
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            // Jwt token oluşturur
            var token = new JwtSecurityToken
                (
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(jwt["AccessTokenExpirationMinutes"]!)),
                signingCredentials: creds
                );
            // jwt formatıyla yazar
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public RefreshToken CreateRefreshToken(string ipAddress)
        {
            var jwt = _configuration.GetSection("Jwt");
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            /*64 bytelik alanı random sayılarla doldurur*/
            return new RefreshToken
            {
                //random sayıları stringe çevirir
                Token = Convert.ToBase64String(randomBytes),
                Expires = DateTime.UtcNow.AddDays(double.Parse(jwt["RefreshTokenExpirationDays"]!)),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress // güvenlik için ip alınmalı
            };
        }
    }
}
