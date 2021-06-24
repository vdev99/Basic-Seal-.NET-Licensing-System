using BasicSealBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicSealBackend.Data.Interface
{
    public interface ISoftwaresRepository
    {
        Task<List<Softwares>> GetSoftwaresByUserId(int userId);
        Task<Softwares> GetSoftwareById(long softwareId);
        Task AddSoftware(Softwares software);
        Task DeleteSoftware(Softwares software);
    }
}
