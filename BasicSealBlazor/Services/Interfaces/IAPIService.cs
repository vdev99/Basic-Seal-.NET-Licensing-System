using BasicSealBlazor.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicSealBlazor.Services.Interfaces
{
    public interface IAPIService
    {
        Task<string> LoginUser(UserAuthDto userAuthDto);
        Task<bool> RegisterUser(UserAuthDto userRegister);
        Task<List<ApplicationsNavDto>> GetUserApps(string token);
        Task<bool> AddApplication(string token, string appName);
        Task<bool> DeleteApplication(string token, long appId);
        Task<SoftwareVersionsDto> GetSoftwareVersions(string token, long appId);
        Task<bool> AddSoftwareVersion(string token, AddSoftwareVersionDto softwareVersion);
        Task<bool> DeleteSoftwareVersion(string token, long versionId);
        Task<bool> GenerateLicenseKey(string token, GenerateLicenseKeyDto generateLicenseKeyParam);
        Task<LicenseRespDto> GetLicenses(string token, GetLicenseReqDto getLicenseReq);
        Task<bool> DeleteSoftwareLicense(string token, long softwareId, long licenseId);
        Task<SoftwareLicenseDto> GetLicenseByKey(string token, long softwareId, string key);
        Task<bool> UpdateSoftwareLicense(string token, SoftwareLicenseDto softwareLicense);
    }
}
