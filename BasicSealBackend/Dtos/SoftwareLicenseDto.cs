using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicSealBackend.Dtos
{
    public class SoftwareLicenseDto
    {
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
    }
}
