using BasicSealBackend.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicSealBackend.Services.Interface
{
    public interface IManagementService
    {
        Task<List<ApplicationsRespDto>> GetSoftwaresByUserId(int userId);
        Task<bool> AddApplication(int userId, string appName);
        Task<bool> DeleteApplication(int userId, int appId);
        Task<SoftwareVersionsRespDto> GetSoftwareVersions(long userId, long softwareId);
        Task<bool> AddSoftwareVersion(int userId, AddSoftwareVersionDto newSoftwareVersion);
        Task<bool> DeleteSoftwareVersion(int userId, int versionId);
        Task<bool> GenerateLicenseKey(int userId, GenerateLicenseKeyParamDto keyParams);
        Task<LicensePaginationRespDto> GetSoftwareLicenses(int userId, GetLicenseReqDto licenseReqDto);
        Task<SoftwareLicenseDto> GetSoftwareLicenseByKey(int userId, int softwareId, string key);
        Task<bool> DeleteSoftwareLicense(int userId, int softwareId, int licenseId);
        Task<bool> UpdateSoftwareLicense(int userId, SoftwareLicenseDto softwareLicense);
    }
}
