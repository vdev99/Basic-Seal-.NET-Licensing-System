using BasicSealClient.Key;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicSealClient.Dtos
{
    internal class LicenseSaveDto
    {
        public LicenseDto licenseData { get; set; }
        public KeyByteSet[] keyBytes { get; set; }
    }
}
