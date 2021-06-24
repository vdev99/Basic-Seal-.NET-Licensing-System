using BasicSealBackend.Data.Interface;
using BasicSealBackend.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicSealBackend.Data
{
    public class SoftwaresRepository : ISoftwaresRepository
    {
        private readonly DataContext _context;

        public SoftwaresRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<List<Softwares>> GetSoftwaresByUserId(int userId)
        {
            var softwares = await _context.Softwares.Where(s => s.UserId == userId && s.isDeleted == false).ToListAsync();

            return softwares;
        }

        public async Task<Softwares> GetSoftwareById(long softwareId)
        {
            var software = await _context.Softwares.FirstOrDefaultAsync(s => s.Id == softwareId && s.isDeleted == false);

            return software;
        }

        public async Task AddSoftware(Softwares software)
        {
            await _context.Softwares.AddAsync(software);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSoftware(Softwares software)
        {
            _context.Softwares.Attach(software);

            software.isDeleted = true;

            await _context.SaveChangesAsync();
        }
    }
}
