using BasicSealBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicSealBackend.Data.Interface
{
    public interface IUsersRepository
    {
        Task<Users> GetUserByEmail(string email);

        Task SaveUser(Users user);
    }
}
