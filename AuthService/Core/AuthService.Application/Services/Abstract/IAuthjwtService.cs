using AuthService.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Application.Services.Abstract
{
    public interface IAuthjwtService
    {
         Task<Response<object>> GenerateToken(TokenClaimsDto token);
    }
}
