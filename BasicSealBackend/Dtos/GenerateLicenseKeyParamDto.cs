using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicSealBackend.Dtos
{
    public class GenerateLicenseKeyParamDto
    {
        public long softwareId { get; set; }
        public int licenseLength { get; set; }
        public bool isLifetime { get; set; }
        public bool enableOfflineVerification { get; set; }
        public int count { get; set; }
    }
}
