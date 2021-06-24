using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicSealBackend.Dtos
{
    public class VerifyResponseDto
    {
        public bool exists { get; set; } = false;
        public bool isFileModified { get; set; } = false;
        public bool isLicenseModified { get; set; } = false;
        public bool isExpired { get; set; } = false;
        public bool isBanned { get; set; } = false;
        public bool hardwareChanged { get; set; } = false;
    }
}
