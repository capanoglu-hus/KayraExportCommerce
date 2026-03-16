using AuthApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthApi.Data
{
    public class UserDbContext : IdentityDbContext<User>
    {
        /*identity kütüphanesi kullanıldığı için user roles gibi tablolar otomatik*/
        public UserDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
