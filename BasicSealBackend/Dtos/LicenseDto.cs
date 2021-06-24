using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicSealBackend.Dtos
{
    public class LicenseDto
    {
        public string version { get; set; }
        public string softwareHash { get; set; }
        public string licenseKey { get; set; }
        public DateTime licenseStartDate { get; set; }
        public DateTime licenseEndDate { get; set; }
        public bool isLifetime { get; set; }
        public bool isBanned { get; set; }
        public bool enableOfflineVerification { get; set; }
        public string cpuIdentifier { get; set; }
        public string hddIdentifier { get; set; }
        public string systemUUID { get; set; }
    }
}
