using BasicSealBackend.Data.Interface;
using BasicSealBackend.Dtos;
using BasicSealBackend.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bcrypt = BCrypt.Net.BCrypt;
using AutoMapper;
using BasicSealBackend.Models;

namespace BasicSealBackend.Services
{
    public class AuthService : IAuthService
    {
        private readonly IMapper _mapper;
        private readonly IUsersRepository _usersRepository;

        public AuthService(IMapper mapper, IUsersRepository usersRepository)
        {
            _mapper = mapper;
            _usersRepository = usersRepository;
        }

        public async Task<long?> VerifyUser(UserAuthDto userLoginDto) //returns user id
        {
            var userResult = await _usersRepository.GetUserByEmail(userLoginDto.Email);

            if(userResult != null && Bcrypt.Verify(userLoginDto.Password, userResult.Password))
            {
                return userResult.Id;
            }

            return null;
        }

        public async Task<bool> CheckMail(string email)
        {
            var result = await _usersRepository.GetUserByEmail(email);

            return result == null ? false : true;
        }

        public async Task RegisterUser(UserAuthDto userRegister)
        {
            string passwordHash = Bcrypt.HashPassword(userRegister.Password);

            Users newUser = new Users { Email = userRegister.Email, Password = passwordHash };

            await _usersRepository.SaveUser(newUser);
        }
    }
}
