using BasicSealBackend.Data.Interface;
using BasicSealBackend.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BasicSealBackend.Dtos;

namespace BasicSealBackend.Data
{
    public class SoftwareLicensesRepository :  ISoftwareLicensesRepository
    {
        private readonly DataContext _context;

        public SoftwareLicensesRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<SoftwareLicenses> GetLicenseByKey(string licenseKey)
        {
            var license = await _context.SoftwareLicenses.FirstOrDefaultAsync(k => k.LicenseKey == licenseKey && k.isDeleted == false);

            return license;
        }

        public async Task<List<SoftwareLicenses>> GetLicensesByKey(string licenseKey)
        {
            var licenses = await _context.SoftwareLicenses.Where(k => k.LicenseKey == licenseKey && k.isDeleted == false).AsNoTracking().ToListAsync();

            return licenses;
        }

        public async Task Update(SoftwareLicenses softwareLicense)
        {
            _context.SoftwareLicenses.Update(softwareLicense);
            await _context.SaveChangesAsync();
        }

        public async Task SaveLicense(List<SoftwareLicenses> softwareLicenses)
        {
            await _context.SoftwareLicenses.AddRangeAsync(softwareLicenses);
            await _context.SaveChangesAsync();
        }

        public async Task<(List<SoftwareLicenses> softLicenses, int licenseCount)> GetSoftwareLicenses(int softwareId, int pageNumber, int pageSize, LicenseFiltersDto filters)
        {
            var licenses =  _context.SoftwareLicenses
                                .Where(l => l.SoftwareId == softwareId && l.isDeleted == false);

            if(filters != null)
            {
                if(filters.length.Count != 0)
                {
                    licenses = licenses.Where(l => filters.length.Contains(l.LicenseLength));
                }
                if (filters.active.HasValue)
                {
                    licenses = licenses.Where(l => l.IsActivated == filters.active);
                }
                if (filters.banned.HasValue)
                {
                    licenses = licenses.Where(l => l.IsBanned == filters.banned);
                }
                if (filters.expired.HasValue)
                {
                    if (filters.expired.Value)
                    {
                        licenses = licenses.Where(l => DateTime.UtcNow > l.LicenseEndDate && l.LicenseEndDate != DateTime.MinValue);
                    }
                    else
                    {
                        licenses = licenses.Where(l => DateTime.UtcNow < l.LicenseEndDate || l.LicenseEndDate == DateTime.MinValue);
                    }
                }
                if (filters.lifetime.HasValue)
                {
                    licenses = licenses.Where(l => l.IsLifetime == filters.lifetime);
                }
            }

            return (await licenses.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(), await licenses.CountAsync());
        }

        public async Task DeleteSoftwareLicense(SoftwareLicenses softwareLicense)
        {
            _context.SoftwareLicenses.Attach(softwareLicense);

            softwareLicense.isDeleted = true;

            await _context.SaveChangesAsync();
        }

        public async Task<SoftwareLicenses> GetLicenseById(long id)
        {
            var license = await _context.SoftwareLicenses.FirstOrDefaultAsync(k => k.Id == id);

            return license;
        }
    }
}
