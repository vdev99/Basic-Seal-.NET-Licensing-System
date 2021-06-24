using BasicSealBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicSealBackend.Data.Interface
{
    public interface ISoftwareVersionsRepository
    {
        Task<SoftwareVersions> GetByVersion(string version, long softwareId);
        Task<SoftwareVersions> GetById(long versionId);
        Task<List<SoftwareVersions>> GetSoftwareVersions(long userId, long softwareId);
        Task AddSoftwareVersion(SoftwareVersions softwareVersion);
        Task DeleteSoftwareVersion(SoftwareVersions softwareVersion);
    }
}
