using AuthService.Application.Dtos;
using AuthService.Application.Helpers;
using AuthService.Application.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Application.Services.Concrate
{
    public class AuthjwtService : IAuthjwtService
    {
        private readonly TokenHelpers _tokenHelpers;

        public AuthjwtService(TokenHelpers tokenHelpers)
        {
            _tokenHelpers = tokenHelpers;
        }

        public async Task<Response<object>> GenerateToken(TokenClaimsDto token)
        {
            try
            {
                var checkUser = token.Email == "admin@admin.com" ? true : false;
                if(checkUser)
                {
                    string result = _tokenHelpers.GenerateToken(token);
                    return new Response<object> { Success = true, Data = result };
                }
                return new Response<object>
                {
                    Data = null,
                    Success = false,
                    Message = "kullanıcı bulunamadı",
                    ErrorCodes = ErrorCodes.Exception
                };
            }
            catch (Exception ex)
            {
                return new Response<object>
                {
                    Data = null,
                    Success = false,
                    Message = "bir hata oluştu",
                    ErrorCodes = ErrorCodes.Exception
                };
            }
            
             

        }
    }
}
