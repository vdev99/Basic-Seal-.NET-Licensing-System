using AutoMapper;
using BasicSealBackend.Data;
using BasicSealBackend.Data.Interface;
using BasicSealBackend.Dtos;
using BasicSealBackend.Key;
using BasicSealBackend.Models;
using BasicSealBackend.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BasicSealBackend.Services
{
    public class ClientService : IClientService
    {
        private readonly IMapper _mapper;

        private readonly ISoftwareLicensesRepository _softwareLicensesRepository;
        private readonly ISoftwareVersionsRepository _softwareVersionsRepository;
        private readonly ISoftwaresRepository _softwaresRepository;
        private readonly IEncryptionService _encryptionService;


        public ClientService(IMapper mapper, ISoftwareLicensesRepository softwareLicensesRepository, 
            ISoftwareVersionsRepository softwareVersionsRepository, ISoftwaresRepository softwaresRepository, IEncryptionService encryptionService)
        {
            _mapper = mapper;
            _softwareLicensesRepository = softwareLicensesRepository;
            _softwareVersionsRepository = softwareVersionsRepository;
            _softwaresRepository = softwaresRepository;
            _encryptionService = encryptionService;
        }


        public async Task<EncryptedLicenseDto> GetLicense(ClientInfoDto clientInfo)
        {
            EncryptedLicenseDto encryptedLicenseDto = new EncryptedLicenseDto();

            var licenses = await _softwareLicensesRepository.GetLicensesByKey(clientInfo.licenseKey);

            SoftwareLicenses license = null;

            foreach(SoftwareLicenses lic in licenses)
            {
                var app = await _softwaresRepository.GetSoftwareById(lic.SoftwareId);

                if(app != null)
                {
                    string currentLicAppId = _encryptionService.HashString(lic.SoftwareId + app.SoftwareName)
                                                        .ToUpper().Substring(0, 10);

                    license = currentLicAppId == clientInfo.softwareId ? lic : null;
                }
            }

            if (license == null) { return encryptedLicenseDto; }

            encryptedLicenseDto.licenseExists = true;

            var softwareVersion = await _softwareVersionsRepository.GetByVersion(clientInfo.version, license.SoftwareId);
            if(softwareVersion == null) { encryptedLicenseDto.isFileModified = true; return encryptedLicenseDto; }
            if (softwareVersion.Hash != clientInfo?.softwareHash) { encryptedLicenseDto.isFileModified = true; return encryptedLicenseDto; }

            if (license.IsActivated) 
            {
                if (DateTime.Compare(license.LicenseEndDate, DateTime.UtcNow) < 0) { encryptedLicenseDto.isExpired = true; return encryptedLicenseDto; }
                if (license.IsBanned) { encryptedLicenseDto.isBanned = true; return encryptedLicenseDto; }

                var activeLicenseDto = _mapper.Map<LicenseDto>(license);

                activeLicenseDto.version = clientInfo.version;
                activeLicenseDto.softwareHash = softwareVersion.Hash;

                ExecuteLicenseEncryption(ref encryptedLicenseDto, softwareVersion.Hash, activeLicenseDto);

                return encryptedLicenseDto;
            }

            license.LicenseStartDate = DateTime.UtcNow;
            license.LicenseEndDate = DateTime.UtcNow.AddDays(license.LicenseLength);
            license.CpuIdentifier = clientInfo.cpuIdentifier;
            license.HddIdentifier = clientInfo.hddIdentifier;
            license.SystemUUID = clientInfo.systemUUID;
            license.IsActivated = true;

            await _softwareLicensesRepository.Update(license);

            var licenseDto = _mapper.Map<LicenseDto>(license);

            licenseDto.version = clientInfo.version;
            licenseDto.softwareHash = softwareVersion.Hash;

            ExecuteLicenseEncryption(ref encryptedLicenseDto, softwareVersion.Hash, licenseDto);

            return encryptedLicenseDto;
        }

        private void ExecuteLicenseEncryption(ref EncryptedLicenseDto encryptedLicenseDto, string applicationHash, LicenseDto license)
        {
            string licenseJson = JsonSerializer.Serialize(license);

            KeyByteSet[] keySet = _encryptionService.GenerateKeyByteSet();
            int seed = _encryptionService.generatePkvSeed(applicationHash, licenseJson);
            string encryptionKey = _encryptionService.GeneratePkvKey(seed, keySet);

            byte[] salt = _encryptionService.GenerateRandomBytes(8);

            KeyByteSet[] clientKeySet = new KeyByteSet[]
            {
                    keySet[0],
                    keySet[2],
                    keySet[4],
                    keySet[6]
            };

            string licenseObjJson = JsonSerializer.Serialize( new { keyBytes = clientKeySet, licenseData = license });
            byte[] encryptedLicense = _encryptionService.AESEncrypt(encryptionKey, Encoding.ASCII.GetBytes(licenseObjJson), salt);

            byte[] encryptedLicenseWithSalt = new byte[salt.Length + encryptedLicense.Length];
            Buffer.BlockCopy(salt, 0, encryptedLicenseWithSalt, 0, salt.Length);
            Buffer.BlockCopy(encryptedLicense, 0, encryptedLicenseWithSalt, salt.Length, encryptedLicense.Length);

            encryptedLicenseDto.licenseData = Convert.ToBase64String(encryptedLicenseWithSalt);
            encryptedLicenseDto.key = encryptionKey;
        }

        public async Task<VerifyResponseDto> VerifyLicense(VerifyLicenseDto clientInfo)
        {
            VerifyResponseDto respDto = new VerifyResponseDto();

            var licenses = await _softwareLicensesRepository.GetLicensesByKey(clientInfo.license.licenseKey);

            SoftwareLicenses license = null;

            foreach (SoftwareLicenses lic in licenses)
            {
                var app = await _softwaresRepository.GetSoftwareById(lic.SoftwareId);

                if (app != null)
                {
                    string currentLicAppId = _encryptionService.HashString(lic.SoftwareId + app.SoftwareName)
                                                        .ToUpper().Substring(0, 10);

                    license = currentLicAppId == clientInfo.softwareId ? lic : null;
                }
            }

            if (license == null) { return respDto; }
            if(license.IsActivated == false) { return respDto; }

            respDto.exists = true;

            var softwareVersion = await _softwareVersionsRepository.GetByVersion(clientInfo.license.version, license.SoftwareId);

            if(softwareVersion == null) { respDto.isFileModified = true; return respDto; }
            if (softwareVersion.Hash != clientInfo.license.softwareHash) { respDto.isFileModified = true; return respDto; }

            int hardwareChange = 0;
            hardwareChange = license.CpuIdentifier == clientInfo.license.cpuIdentifier ? hardwareChange+=1 : hardwareChange;
            hardwareChange = license.HddIdentifier == clientInfo.license.hddIdentifier ? hardwareChange+=1 : hardwareChange;
            hardwareChange = license.SystemUUID == clientInfo.license.systemUUID ? hardwareChange+=1 : hardwareChange;

            if(hardwareChange < 2) { respDto.hardwareChanged = true; return respDto; }
            if (license.IsBanned) { respDto.isBanned = true; return respDto; }
            if (license.IsLifetime) { return respDto; }

            respDto.isLicenseModified = CheckIfLicenseModified() ? true : false;

            if (DateTime.Compare(license.LicenseEndDate, DateTime.UtcNow) >= 0) { return respDto; }
            else { respDto.isExpired = true; return respDto; }

            bool CheckIfLicenseModified()
            {
                if (clientInfo.license.softwareHash != softwareVersion.Hash) 
                { return true; }
                if (clientInfo.license.licenseStartDate.ToString() != license.LicenseStartDate.ToString())
                { return true; }
                if (clientInfo.license.licenseEndDate.ToString() != license.LicenseEndDate.ToString())
                { return true; }
                if (clientInfo.license.isLifetime != license.IsLifetime)
                { return true; }
                if (clientInfo.license.isBanned != license.IsBanned)
                { return true; }
                if (clientInfo.license.enableOfflineVerification != license.enableOfflineVerification)
                { return true; }
                if (clientInfo.license.cpuIdentifier != license.CpuIdentifier)
                { return true; }
                if (clientInfo.license.hddIdentifier != license.HddIdentifier)
                { return true; }
                if (clientInfo.license.systemUUID != license.SystemUUID)
                { return true; }

                return false;
            }
        }

    }
}
