using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Application.Dtos
{
    public class TokenClaimsDto
    {
        public string Id { get; set; }
        public string Email { get; set; }

        public string Role { get; set; }
    }
}
