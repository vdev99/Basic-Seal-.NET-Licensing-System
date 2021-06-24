using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicSealBackend.Dtos
{
    public class VerifyLicenseDto
    {
        public string softwareId { get; set; }
        public LicenseDto license { get; set; }
    }
}
