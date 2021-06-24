using BasicSealBackend.Key;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicSealBackend.Dtos
{
    public class EncryptedLicenseDto
    {
        public string key { get; set; }
        public string licenseData { get; set; }

        public bool licenseExists { get; set; } = false;
        public bool isFileModified { get; set; } = false;
        public bool isExpired { get; set; } = false;
        public bool isBanned { get; set; } = false;
    }
}
