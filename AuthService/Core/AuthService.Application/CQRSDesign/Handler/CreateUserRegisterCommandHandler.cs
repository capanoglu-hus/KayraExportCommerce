using AuthService.Application.CQRSDesign.Commands;
using AuthService.Persistence.Context;
using AuthService.Persistence.Identity;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Application.CQRSDesign.Handler
{
    public class CreateUserRegisterCommandHandler 
    {
        private readonly AuthContext _context;
        private readonly UserManager<AppUser> _manager;
        

        public CreateUserRegisterCommandHandler(AuthContext context , UserManager<AppUser> manager)
        {
            _context = context;
            _manager = manager;
           
        }

        public async Task Handle(CreateUserRegisterCommand command )
        {
            var user = new AppUser()
            {
                Name = command.Name,
                Surname = command.Surname,
                Email = command.Email,
                UserName = command.Username
            };
            await _manager.CreateAsync(user, command.Password);
        }
    }
}
