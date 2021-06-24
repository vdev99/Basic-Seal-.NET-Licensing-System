using BasicSealBackend.Dtos;
using BasicSealBackend.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using BasicSealBackend.Helpers;
using System.Linq;
using System.Threading.Tasks;

namespace BasicSealBackend.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ManagementController : ControllerBase
    {
        private readonly IManagementService _managementService;

        public ManagementController(IManagementService managementService)
        {
            _managementService = managementService;
        }

        [HttpGet("software")]
        public async Task<List<ApplicationsRespDto>> GetApplications()
        {
            int userId = new JwtSecurityToken().GetUserId(Request);

            return await _managementService.GetSoftwaresByUserId(userId);
        }

        [HttpPost("software/new")]
        public async Task<IActionResult> AddApplication([FromQuery] string appName)
        {
            int userId = new JwtSecurityToken().GetUserId(Request);

            bool isSuccess = await _managementService.AddApplication(userId, appName);

            if (isSuccess) { return Ok(); }

            return Forbid();
        }

        [HttpDelete("software")]
        public async Task<IActionResult> deleteApplication([FromQuery] int appId)
        {
            int userId = new JwtSecurityToken().GetUserId(Request);

            bool isSuccess = await _managementService.DeleteApplication(userId, appId);

            if (isSuccess) { return Ok(); }

            return Forbid();
        }

        [HttpGet("software/versions")]
        public async Task<SoftwareVersionsRespDto> GetSoftwareVersions([FromQuery] long appId)
        {
            int userId = new JwtSecurityToken().GetUserId(Request);

            return await _managementService.GetSoftwareVersions(userId, appId);
        }

        [HttpPost("software/versions/new")]
        public async Task<IActionResult> AddSoftwareVersion([FromBody] AddSoftwareVersionDto softwareVersion)
        {
            int userId = new JwtSecurityToken().GetUserId(Request);

            bool isSuccess = await _managementService.AddSoftwareVersion(userId, softwareVersion);

            if (isSuccess) { return Ok(); }

            return Forbid();
        }

        [HttpDelete("software/versions")]
        public async Task<IActionResult> DeleteSoftwareVersion([FromQuery] int versionId)
        {
            int userId = new JwtSecurityToken().GetUserId(Request);

            bool isSuccess = await _managementService.DeleteSoftwareVersion(userId, versionId);

            if (isSuccess) { return Ok(); }

            return Forbid();
        }

        [HttpPost("licenses/generateKey")]
        public async Task<IActionResult> GenerateLicenseKey([FromBody] GenerateLicenseKeyParamDto keyParameters)
        {
            int userId = new JwtSecurityToken().GetUserId(Request);

            bool isSuccess = await _managementService.GenerateLicenseKey(userId, keyParameters);

            if (isSuccess) { return Ok(); }

            return Forbid();
        }

        [HttpPost("licenses")]
        public async Task<LicensePaginationRespDto> GetSoftwareLicenses([FromBody] GetLicenseReqDto getLicenseReq)
        {
            int userId = new JwtSecurityToken().GetUserId(Request);

            var result =  await _managementService.GetSoftwareLicenses(userId, getLicenseReq);

            return result;
        }

        [HttpGet("licenses")]
        public async Task<SoftwareLicenseDto> GetSoftwareLicenseByKey([FromQuery] int softwareId, string key)
        {
            int userId = new JwtSecurityToken().GetUserId(Request);

            var result = await _managementService.GetSoftwareLicenseByKey(userId, softwareId, key);

            return result;
        }

        [HttpDelete("licenses")]
        public async Task<IActionResult> DeleteSoftwareLicense([FromQuery] int softwareId, int licenseId)
        {
            int userId = new JwtSecurityToken().GetUserId(Request);

            var isSuccess = await _managementService.DeleteSoftwareLicense(userId, softwareId, licenseId);

            if (isSuccess) { return Ok(); }

            return Forbid();
        }

        [HttpPost("licenses/update")]
        public async Task<IActionResult> UpdateSoftwareLicense([FromBody] SoftwareLicenseDto softwareLicense)
        {
            int userId = new JwtSecurityToken().GetUserId(Request);

            bool isSuccess = await _managementService.UpdateSoftwareLicense(userId, softwareLicense);

            if (isSuccess) { return Ok(); }

            return Forbid();
        }
    }
}
