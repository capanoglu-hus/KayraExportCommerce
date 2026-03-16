using Microsoft.AspNetCore.Identity;

namespace AuthService.Persistence.Identity
{
    public class AppUser :IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }

    }
}
