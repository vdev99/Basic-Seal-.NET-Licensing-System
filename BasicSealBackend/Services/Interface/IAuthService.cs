using BasicSealBackend.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicSealBackend.Services.Interface
{
    public interface IAuthService
    {
        Task<long?> VerifyUser(UserAuthDto userLoginDto);

        Task<bool> CheckMail(string email);

        Task RegisterUser(UserAuthDto userRegister);
    }
}
