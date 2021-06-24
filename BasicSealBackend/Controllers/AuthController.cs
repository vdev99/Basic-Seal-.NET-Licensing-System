using BasicSealBackend.Dtos;
using BasicSealBackend.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace BasicSealBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IAuthService _authService;

        public AuthController(IConfiguration config, IAuthService authService)
        {
            _config = config;
            _authService = authService;
        }

        [HttpPost("users/login")]
        public async Task<IActionResult> Login([FromBody] UserAuthDto userLogin)
        {
            long? userID = await _authService.VerifyUser(userLogin);

            if (userID != null)
            {
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, userLogin.Email),
                    new Claim(ClaimTypes.NameIdentifier, userID.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                var signKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:JWT:Secret").Value));
                var signingCredentials = new SigningCredentials(signKey, SecurityAlgorithms.HmacSha512Signature);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(authClaims),
                    Expires = DateTime.Now.AddDays(60),
                    SigningCredentials = signingCredentials,
                    Issuer = _config["AppSettings:JWT:ValidIssuer"]
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var mToken = tokenHandler.CreateToken(tokenDescriptor);

                return Ok(new { token = tokenHandler.WriteToken(mToken) });
            }

            return Unauthorized();
        }

        [HttpPost("users/register")]
        public async Task<IActionResult> Register([FromBody] UserAuthDto userRegister)
        {
            bool isEmailRegistered = await _authService.CheckMail(userRegister.Email);

            if (isEmailRegistered)
            {
                return Forbid();
            }
            else
            {
                await _authService.RegisterUser(userRegister);
            }

            return Ok();
        }
    }
}
