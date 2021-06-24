using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicSealBlazor.Dtos
{
    public class GenerateLicenseKeyDto
    {
        public long softwareId { get; set; }
        public int licenseLength { get; set; }
        public bool isLifetime { get; set; }
        public bool enableOfflineVerification { get; set; }
        public int count { get; set; }
    }
}
