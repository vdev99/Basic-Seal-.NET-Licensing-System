using BasicSealBackend.Data.Interface;
using BasicSealBackend.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicSealBackend.Data
{
    public class UsersRepository : IUsersRepository
    { 
        private readonly DataContext _context;

        public UsersRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<Users> GetUserByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(e => e.Email == email);
        }

        public async Task SaveUser(Users user)
        {
            await _context.AddAsync(user);
            await _context.SaveChangesAsync();
        }

    }
}
