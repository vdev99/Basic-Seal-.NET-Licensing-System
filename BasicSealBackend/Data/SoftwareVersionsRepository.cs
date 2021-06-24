using BasicSealBackend.Data.Interface;
using BasicSealBackend.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicSealBackend.Data
{
    public class SoftwareVersionsRepository : ISoftwareVersionsRepository
    {
        private readonly DataContext _context;

        public SoftwareVersionsRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<SoftwareVersions> GetByVersion(string version, long softwareId)
        {
            var softVersion = await _context.SoftwareVersions.FirstOrDefaultAsync
                                            (s => s.Version == version && s.SoftwareId == softwareId && s.isDeleted == false);

            return softVersion;
        }

        public async Task<SoftwareVersions> GetById(long versionId)
        {
            var softVersion = await _context.SoftwareVersions.FirstOrDefaultAsync(s => s.Id == versionId && s.isDeleted == false);

            return softVersion;
        }

        public async Task<List<SoftwareVersions>> GetSoftwareVersions(long userId, long softwareId)
        {
            var softVersion = await _context.SoftwareVersions.Where
                                                (s => s.SoftwareId == softwareId && s.UserId == userId && s.isDeleted == false).ToListAsync();

            return softVersion;
        }

        public async Task AddSoftwareVersion(SoftwareVersions softwareVersion)
        {
            await _context.SoftwareVersions.AddAsync(softwareVersion);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSoftwareVersion(SoftwareVersions softwareVersion)
        {
            _context.SoftwareVersions.Attach(softwareVersion);

            softwareVersion.isDeleted = true;

            await _context.SaveChangesAsync();
        }
    }
}
