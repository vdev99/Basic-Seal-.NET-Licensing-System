using AutoMapper;
using BasicSealBackend.Data.Interface;
using BasicSealBackend.Dtos;
using BasicSealBackend.Models;
using BasicSealBackend.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace BasicSealBackend.Services
{
    public class ManagementService : IManagementService
    {
        private readonly IMapper _mapper;

        private readonly ISoftwaresRepository _softwaresRepository;
        private readonly ISoftwareVersionsRepository _softwareVersionsRepository;
        private readonly IEncryptionService _encryptionService;
        private readonly ISoftwareLicensesRepository _softwareLicensesRepository;

        public ManagementService(ISoftwaresRepository softwaresRepository, IMapper mapper, 
            ISoftwareVersionsRepository softwareVersionsRepository, IEncryptionService encryptionService,
            ISoftwareLicensesRepository softwareLicensesRepository)
        {
            _mapper = mapper;
            _softwaresRepository = softwaresRepository;
            _softwareVersionsRepository = softwareVersionsRepository;
            _encryptionService = encryptionService;
            _softwareLicensesRepository = softwareLicensesRepository;
        }

        public async Task<List<ApplicationsRespDto>> GetSoftwaresByUserId(int userId)
        {
            var softwares = await _softwaresRepository.GetSoftwaresByUserId(userId);

            return _mapper.Map<List<ApplicationsRespDto>>(softwares);
        }

        public async Task<bool> AddApplication(int userId, string appName)
        {
            var software = _softwaresRepository.GetSoftwaresByUserId(userId)
                                                        .Result.FirstOrDefault(s => s.SoftwareName == appName);

            if (software != null)
            {
                return false;
            }
            else
            {
                var newSoftware = new Softwares { UserId = userId, SoftwareName = appName };
                await _softwaresRepository.AddSoftware(newSoftware);

                return true;
            }
        }

        public async Task<bool> DeleteApplication(int userId, int appId)
        {
            var software = _softwaresRepository.GetSoftwaresByUserId(userId)
                                                        .Result.FirstOrDefault(s => s.Id == appId);
            if(software != null)
            {
                await _softwaresRepository.DeleteSoftware(software);

                return true;
            }

            return false;
        }

        public async Task<SoftwareVersionsRespDto> GetSoftwareVersions(long userId, long softwareId)
        {
            var softwareVersions = await _softwareVersionsRepository.GetSoftwareVersions(userId, softwareId);
            var software = await _softwaresRepository.GetSoftwaresByUserId((int)userId);

            var mappedVersions = _mapper.Map<List<SoftVersion>>(softwareVersions);

            var versions = new SoftwareVersionsRespDto()
            {
                SoftwareId = softwareId,
                SoftwareName = software.Where(s => s.Id == softwareId).FirstOrDefault().SoftwareName,
                version = mappedVersions
            };

            versions.generatedSoftwareId = _encryptionService.HashString(versions.SoftwareId + software.FirstOrDefault(s => s.Id == versions.SoftwareId).SoftwareName)
                                                        .ToUpper().Substring(0, 10);

            return versions;
        }

        public async Task<bool> AddSoftwareVersion(int userId, AddSoftwareVersionDto newSoftwareVersion)
        {
            var software =  _softwaresRepository.GetSoftwaresByUserId(userId)
                                                        .Result.FirstOrDefault(s => s.Id == newSoftwareVersion.SoftwareId);

            if (software != null)
            {
                var softwareVersion = await _softwareVersionsRepository.GetByVersion(newSoftwareVersion.Version, newSoftwareVersion.SoftwareId);

                if(softwareVersion == null)
                {
                    var addSoftwareVersion = new SoftwareVersions 
                    {
                        SoftwareId = newSoftwareVersion.SoftwareId, 
                        UserId = userId,
                        Version = newSoftwareVersion.Version,
                        Hash = newSoftwareVersion.Hash
                    };

                    await _softwareVersionsRepository.AddSoftwareVersion(addSoftwareVersion);

                    return true;
                }
            }
            
            return false;
        }

        public async Task<bool> DeleteSoftwareVersion(int userId, int versionId)
        {
            var softwareVersion = await _softwareVersionsRepository.GetById(versionId);

            if (softwareVersion != null && softwareVersion.UserId == userId)
            {
                await _softwareVersionsRepository.DeleteSoftwareVersion(softwareVersion);

                return true;
            }

            return false;
        }

        public async Task<bool> GenerateLicenseKey(int userId, GenerateLicenseKeyParamDto keyParams)
        {

            var userApp = _softwaresRepository.GetSoftwaresByUserId(userId)
                                                    .Result.FirstOrDefault(s => s.Id == keyParams.softwareId);

            if(userApp != null)
            {
                List<SoftwareLicenses> licenses = new List<SoftwareLicenses>();

                string key = _encryptionService.GenerateLicenseKey(16);

                for (int i = 0; i < keyParams.count; i++)
                {
                    licenses.Add(new SoftwareLicenses
                    {
                        SoftwareId = keyParams.softwareId,
                        LicenseLength = keyParams.isLifetime ? 0 : keyParams.licenseLength,
                        IsLifetime = keyParams.isLifetime,
                        enableOfflineVerification = keyParams.enableOfflineVerification,
                        LicenseKey = _encryptionService.GenerateLicenseKey(16)
                    });
                }

                await _softwareLicensesRepository.SaveLicense(licenses);

                return true;
            }

            return false;
        }

        public async Task<LicensePaginationRespDto> GetSoftwareLicenses(int userId, GetLicenseReqDto licenseReqDto)
        {
            var userSoftware = _softwaresRepository.GetSoftwaresByUserId(userId).Result
                                                            .FirstOrDefault(s => s.Id == licenseReqDto.softwareId);
            if(userSoftware != null)
            {
               var result = await _softwareLicensesRepository
                                    .GetSoftwareLicenses(licenseReqDto.softwareId, licenseReqDto.pageNumber, licenseReqDto.pageSize, licenseReqDto.filters);

                var softwareLicenses = _mapper.Map<List<SoftwareLicenseDto>>(result.softLicenses);

                LicensePaginationRespDto licensePaginationRespDto = new LicensePaginationRespDto
                {
                    softwareId = licenseReqDto.softwareId,
                    pageNumber = licenseReqDto.pageNumber,
                    pageSize = licenseReqDto.pageSize,
                    totalRecords = result.licenseCount,
                    
                    softwareLicenseDtos = softwareLicenses
                };

                return licensePaginationRespDto;
            }

            return null;
        }

        public async Task<SoftwareLicenseDto> GetSoftwareLicenseByKey(int userId, int softwareId, string key)
        {
            var userSoftware = _softwaresRepository.GetSoftwaresByUserId(userId).Result
                                                            .FirstOrDefault(s => s.Id == softwareId);

            if(userSoftware != null)
            {
                var license = await _softwareLicensesRepository.GetLicenseByKey(key);

                return _mapper.Map<SoftwareLicenseDto>(license);
            }

            return null;
        }


        public async Task<bool> DeleteSoftwareLicense(int userId, int softwareId, int licenseId)
        {
            var userSoftware = _softwaresRepository.GetSoftwaresByUserId(userId).Result
                                                            .FirstOrDefault(s => s.Id == softwareId);

            if (userSoftware != null)
            {
                var license = await _softwareLicensesRepository.GetLicenseById(licenseId);

                if(license != null)
                {
                    await _softwareLicensesRepository.DeleteSoftwareLicense(license);

                    return true;
                }
            }

            return false;
        }

        public async Task<bool> UpdateSoftwareLicense(int userId, SoftwareLicenseDto softwareLicense)
        {
            var userSoftware = _softwaresRepository.GetSoftwaresByUserId(userId).Result
                                                            .FirstOrDefault(s => s.Id == softwareLicense.SoftwareId);

            if (userSoftware != null)
            {
                var licenses = await _softwareLicensesRepository.GetLicensesByKey(softwareLicense.LicenseKey);

                foreach(SoftwareLicenses lic in licenses)
                {
                    if(lic.LicenseKey == softwareLicense.LicenseKey && lic.Id != softwareLicense.Id)
                    {
                        return false;
                    }
                }

                await _softwareLicensesRepository.Update(_mapper.Map<SoftwareLicenses>(softwareLicense));

                return true;
            }

            return false;
        }
    }
}
