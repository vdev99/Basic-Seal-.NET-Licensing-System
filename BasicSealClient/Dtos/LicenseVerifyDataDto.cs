using System;
using System.Collections.Generic;
using System.Text;

namespace BasicSealClient.Dtos
{
    internal class LicenseVerifyDataDto
    {
        public string softwareId { get; set; }
        public LicenseDto license { get; set; }
    }
}
