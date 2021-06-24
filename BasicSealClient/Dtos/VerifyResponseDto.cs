using System;
using System.Collections.Generic;
using System.Text;

namespace BasicSealClient.Dtos
{
    internal class VerifyResponseDto
    {
        public bool exists { get; set; } = false;
        public bool isFileModified { get; set; } = false;
        public bool isLicenseModified { get; set; } = false;
        public bool isExpired { get; set; } = false;
        public bool isBanned { get; set; } = false;
        public bool hardwareChanged { get; set; } = false;
    }
}
