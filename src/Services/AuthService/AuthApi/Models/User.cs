using Microsoft.AspNetCore.Identity;

namespace AuthApi.Models
{
    public class User: IdentityUser
    {
        public List<RefreshToken> RefreshTokens { get; set; } = new();
    }
}
