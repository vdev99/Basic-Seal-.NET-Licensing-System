using BasicSealClient.Key;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicSealClient.Dtos
{
    internal class GetLicenseResponseDto
    {
        public string key { get; set; }
        public string licenseData { get; set; }

        public bool licenseExists { get; set; } = false;
        public bool isFileModified { get; set; } = false;
        public bool isExpired { get; set; } = false;
        public bool isBanned { get; set; } = false;
    }
}
