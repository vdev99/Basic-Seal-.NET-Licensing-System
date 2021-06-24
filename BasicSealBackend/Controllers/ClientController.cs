using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BasicSealBackend.Data;
using BasicSealBackend.Dtos;
using BasicSealBackend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BasicSealBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpPost("licenses/verify")]
        public async Task<VerifyResponseDto> VerifyLicense([FromBody] VerifyLicenseDto license)
        {
            var lic = await _clientService.VerifyLicense(license);
            return lic;
        }

        [HttpPost("licenses/activate")]
        public async Task<EncryptedLicenseDto> GetLicense([FromBody] ClientInfoDto license)
        {
            var licenseResult = await _clientService.GetLicense(license);

            return licenseResult;
        }

        [HttpGet("serverTime")]
        public async Task<IActionResult> GetServerTime()
        {
            return Ok(new { serverTime = DateTime.UtcNow });
        }
    }
}
