using AuthService.Persistence.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Persistence.Context
{
    public class AuthContext :IdentityDbContext<AppUser>
    {
        /*AppUser'u identityden aldığın tablo içine dahil ediyorsun*/
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;initial Catalog=KayraExport;integrated Security=true;Trusted_Connection=True;TrustServerCertificate=true");
        }
        

        
       
    }
}
