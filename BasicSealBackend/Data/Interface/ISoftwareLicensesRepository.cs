using BasicSealBackend.Dtos;
using BasicSealBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicSealBackend.Data.Interface
{
    public interface ISoftwareLicensesRepository
    {
        Task<SoftwareLicenses> GetLicenseByKey(string licenseKey);
        Task<List<SoftwareLicenses>> GetLicensesByKey(string licenseKey);
        Task Update(SoftwareLicenses softwareLicense);
        Task SaveLicense(List<SoftwareLicenses> softwareLicenses);
        Task<(List<SoftwareLicenses> softLicenses, int licenseCount)> GetSoftwareLicenses(int softwareId, int pageNumber, int pageSize, LicenseFiltersDto filters);
        Task DeleteSoftwareLicense(SoftwareLicenses softwareLicense);
        Task<SoftwareLicenses> GetLicenseById(long id);
    }
}
