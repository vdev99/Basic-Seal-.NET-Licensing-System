using System;

namespace BasicSealBlazor.Dtos
{
    public class SoftwareLicenseDto
    {
        public SoftwareLicenseDto()
        {

        }
        public SoftwareLicenseDto(SoftwareLicenseDto softwareLicenseDto)
        {
            Id = softwareLicenseDto.Id;
            SoftwareId = softwareLicenseDto.SoftwareId;
            LicenseKey = softwareLicenseDto.LicenseKey;
            LicenseLength = softwareLicenseDto.LicenseLength;
            LicenseStartDate = softwareLicenseDto.LicenseStartDate;
            LicenseEndDate = softwareLicenseDto.LicenseEndDate;
            IsLifetime = softwareLicenseDto.IsLifetime;
            IsActivated = softwareLicenseDto.IsActivated;
            IsBanned = softwareLicenseDto.IsBanned;
            enableOfflineVerification = softwareLicenseDto.enableOfflineVerification;
            CpuIdentifier = softwareLicenseDto.CpuIdentifier;
            HddIdentifier = softwareLicenseDto.HddIdentifier;
            SystemUUID = softwareLicenseDto.SystemUUID;
        }

        public long Id { get; set; }

        public long SoftwareId { get; set; }

        public string LicenseKey { get; set; }

        public int LicenseLength { get; set; }

        public DateTime LicenseStartDate { get; set; }

        public DateTime LicenseEndDate { get; set; }

        public bool IsLifetime { get; set; }

        public bool IsActivated { get; set; }

        public bool IsBanned { get; set; }

        public bool enableOfflineVerification { get; set; }

        public string CpuIdentifier { get; set; }

        public string HddIdentifier { get; set; }

        public string SystemUUID { get; set; }

        public bool isLoading { get; set; } = false;
    }
}