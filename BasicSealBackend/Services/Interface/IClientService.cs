using BasicSealBackend.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicSealBackend.Services
{
    public interface IClientService
    {
        Task<EncryptedLicenseDto> GetLicense(ClientInfoDto clientInfo);
        Task<VerifyResponseDto> VerifyLicense(VerifyLicenseDto clientLicense);
    }
}
